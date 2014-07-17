/**
 * Copyright (c) 2010, 2011 Darmstadt University of Technology.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 */
package kave.feedback;

import static kave.feedback.TestHelper.assertDirectoryContainsFile;
import static kave.feedback.TestHelper.createRandomFile;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;

import kave.Result;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;

import com.sun.jersey.core.header.FormDataContentDisposition;

public class FeedbackServiceTest {
    @Rule
    public TemporaryFolder temporaryFolder = new TemporaryFolder();
    public File feedbackOutputDirectory;

    private FeedbackService sut;

    @Before
    public void setup() throws IOException {
        File tempRootFolder = temporaryFolder.newFolder();
        feedbackOutputDirectory = new File(tempRootFolder, "data");
        sut = new FeedbackService(feedbackOutputDirectory);
    }

    @Test
    public void indexShouldReturnView() {
        assertNotNull(sut.index());
    }

    @Test
    public void uploadOfExistingFileShouldSucceedAndReturnTrue() throws IOException {
        File fileToUpload = givenARandomFile("test.ext");

        Result expectedResult = Result.ok();
        Result actualResult = whenFileIsUploaded(fileToUpload);

        assertEquals(expectedResult, actualResult);
    }

    @Test
    public void uploadWithCorruptedStreamShouldFail() throws IOException {
        InputStream inputStream = mock(InputStream.class);
        IOException ioException = new IOException("exception cause");
        when(inputStream.read(any(byte[].class))).thenThrow(ioException);

        Result expected = Result.fail(ioException);
        Result actual = sut.upload(inputStream, mockContentDisposition("irrelevant.name"));

        assertEquals(expected.status, actual.status);
        assertTrue(actual.message.startsWith("java.io.IOException: exception cause"));
    }

    @Test
    public void uploadShouldStoreFileInOutputDirectory() throws IOException {
        File fileToUpload = givenARandomFile("testupload.zip");

        whenFileIsUploaded(fileToUpload);

        thenOutputDirectoryContainsFile("00001.zip", fileToUpload);
    }

    @Test
    public void uploadShouldPreserveFileExtension() throws IOException {
        File fileToUpload = givenARandomFile("name.some4random-ext");

        whenFileIsUploaded(fileToUpload);

        thenOutputDirectoryContainsFile("00001.some4random-ext", fileToUpload);
    }

    @Test
    public void uploadShouldPreserveExistingFilesInOutputDirectory() throws IOException {
        File existingFile = givenARandomFileInTheOutputDirectory("00001.zip");
        File fileToUpload = givenARandomFile("new.zip");

        whenFileIsUploaded(fileToUpload);

        thenOutputDirectoryContainsFile("00001.zip", existingFile);
        thenOutputDirectoryContainsFile("00002.zip", fileToUpload);
    }

    @Test
    public void uploadShouldAssignHighestNumberToNewFiles() throws IOException {
        givenARandomFileInTheOutputDirectory("00005.ext");
        givenARandomFileInTheOutputDirectory("00023.zip");
        File fileToUpload = givenARandomFile("blablub.foo");

        whenFileIsUploaded(fileToUpload);

        thenOutputDirectoryContainsFile("00024.foo", fileToUpload);
    }

    @Test
    public void uploadShouldNotFailWhenOuputDirectoryContainsGarbage() throws IOException {
        File fileToUpload = givenARandomFile("data.zip");
        givenARandomFileInTheOutputDirectory("garbage.zip");

        Result expected = Result.ok();
        Result actual = whenFileIsUploaded(fileToUpload);

        assertEquals(expected, actual);
    }

    private File givenARandomFileInTheOutputDirectory(String fileName) throws IOException {
        File randomFile = givenARandomFile(fileName);
        File randomFileInOutputDirectory = new File(feedbackOutputDirectory, fileName);
        FileUtils.copyFile(randomFile, randomFileInOutputDirectory);
        return randomFileInOutputDirectory;
    }

    private File givenARandomFile(String fileName) throws IOException {
        return createRandomFile(temporaryFolder, fileName);
    }

    private Result whenFileIsUploaded(File fileToUpload) throws FileNotFoundException {
        String fileName = fileToUpload.getName();
        FormDataContentDisposition contentDisposition = mockContentDisposition(fileName);
        InputStream fileInputStream = null;
        try {
            fileInputStream = new FileInputStream(fileToUpload);
            return sut.upload(fileInputStream, contentDisposition);
        } finally {
            IOUtils.closeQuietly(fileInputStream);
        }
    }

    private FormDataContentDisposition mockContentDisposition(String fileName) {
        FormDataContentDisposition contentDisposition = mock(FormDataContentDisposition.class);
        when(contentDisposition.getFileName()).thenReturn(fileName);
        return contentDisposition;
    }

    private void thenOutputDirectoryContainsFile(final String expectedFileName, File expectedFileContent)
            throws IOException {
        assertDirectoryContainsFile(feedbackOutputDirectory, expectedFileName, expectedFileContent);
    }
}