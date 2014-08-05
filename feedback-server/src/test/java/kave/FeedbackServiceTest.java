/**
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package kave;

import static kave.FeedbackService.NO_SINGLE_UPLOAD;
import static kave.FeedbackService.UPLOAD_FAILED;
import static kave.FeedbackServiceTest.ValidationResult.ERROR;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;
import static org.mockito.Matchers.any;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Map;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;
import org.mockito.invocation.InvocationOnMock;
import org.mockito.stubbing.Answer;

public class FeedbackServiceTest {

    private static final String KAVE_ERROR_MESSAGE = "KAVE_ERROR_MESSAGE";

    enum ValidationResult {
        OK, KAVE_ERROR, ERROR
    }

    @Rule
    public TemporaryFolder temporaryFolder = new TemporaryFolder();

    private File dataDir;
    private File tmpDir;

    private ValidationResult validationResult;
    private UploadCleanser checker;

    private FeedbackServiceFixture fix;
    private FeedbackService sut;

    private UniqueFileCreator tmpUfc;
    private UniqueFileCreator dataUfc;

    @Before
    public void setup() throws IOException {
        fix = new FeedbackServiceFixture(temporaryFolder.getRoot());

        dataDir = fix.getDataFolder();
        tmpDir = fix.getTempFolder();

        mockUploadCleanser();

        tmpUfc = mockFileCreator(tmpDir);
        dataUfc = mockFileCreator(dataDir);

        sut = new FeedbackService(dataDir, tmpDir, checker, tmpUfc, dataUfc);
    }

    private void mockUploadCleanser() throws IOException {
        validationResult = ValidationResult.OK;
        checker = mock(UploadCleanser.class);
        when(checker.purify(any(File.class))).thenAnswer(new Answer<File>() {
            @Override
            public File answer(InvocationOnMock invocation) throws Throwable {
                switch (validationResult) {
                case ERROR:
                    throw new RuntimeException("GENERAL_ERROR_MESSAGE");
                case KAVE_ERROR:
                    throw new KaVEException(KAVE_ERROR_MESSAGE);
                default:
                    return (File) invocation.getArguments()[0];
                }
            }
        });
    }

    private UniqueFileCreator mockFileCreator(final File base) throws IOException {
        final int[] fileNum = new int[] { 10 };
        UniqueFileCreator ufc = mock(UniqueFileCreator.class);
        when(ufc.createNextUniqueFile()).thenAnswer(new Answer<File>() {
            @Override
            public File answer(InvocationOnMock invocation) throws Throwable {
                String fileName = (fileNum[0]++) + ".zip";
                return new File(base, fileName);
            }
        });
        return ufc;
    }

    @Test
    public void itsNotAnIssueIfFoldersArePreExisting() throws IOException {
        dataDir.mkdir();
        File d = new File(dataDir, "d.txt");
        d.createNewFile();

        tmpDir.mkdir();
        File t = new File(dataDir, "t.txt");
        t.createNewFile();

        sut = new FeedbackService(dataDir, tmpDir, checker, tmpUfc, dataUfc);

        assertTrue(d.exists());
        assertTrue(t.exists());
    }

    @Test
    public void indexShouldReturnView() {
        assertNotNull(sut.index());
    }

    @Test
    public void uploadingMultipleFilesFails() throws FileNotFoundException {
        Result actual = sut.upload(fix.createMultiFileUpload());
        Result expected = Result.fail(NO_SINGLE_UPLOAD);
        assertEquals(expected, actual);
    }

    @Test
    public void failingValidationCreateError() throws FileNotFoundException {
        validationResult = ERROR;
        Result actual = sut.upload(fix.createZipFileUpload());
        Result expected = Result.fail(FeedbackService.UPLOAD_FAILED);
        assertEquals(expected, actual);
    }

    @Test
    public void failingValidationCreateKaveError() throws FileNotFoundException {
        validationResult = ValidationResult.KAVE_ERROR;
        Result actual = sut.upload(fix.createZipFileUpload());
        Result expected = Result.fail(KAVE_ERROR_MESSAGE);
        assertEquals(expected, actual);
    }

    @Test
    public void happyPathForUpload() throws IOException {
        Result actual = sut.upload(fix.createZipFileUpload());
        Result expected = Result.ok();
        assertEquals(expected, actual);

        verify(checker).purify(any(File.class));
        verify(tmpUfc).createNextUniqueFile();
        verify(dataUfc).createNextUniqueFile();
    }

    @Test
    public void uploadCreatesCorrectFile() throws IOException {
        sut.upload(fix.createZipFileUpload());
        assertDirectoryContainsZipFile(dataDir, "10.zip", fix.getZipFile());
        assertDirectoryDoesNotContainFile(tmpDir, "10.zip");
    }

    @Test
    public void uploadWithCorruptedStreamShouldFail() throws IOException {
        Result actual = sut.upload(fix.createFailingUpload());
        Result expected = Result.fail(UPLOAD_FAILED);
        assertEquals(expected, actual);
    }

    public static void assertDirectoryContainsZipFile(File directory, final String expectedFileName,
            File expectedFileContent) throws IOException {
        File actualFile = new File(directory, expectedFileName);
        assertTrue(actualFile.exists());

        Map<String, String> expecteds = UploadCleanserTest.readZipFile(expectedFileContent);
        Map<String, String> actuals = UploadCleanserTest.readZipFile(actualFile);
        assertEquals(expecteds, actuals);
    }

    private static void assertDirectoryDoesNotContainFile(File dir, String fileName) {
        assertFalse(new File(dir, fileName).exists());
    }
}