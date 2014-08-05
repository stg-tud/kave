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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */
package kave;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import java.io.Closeable;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.charset.Charset;
import java.util.Enumeration;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipOutputStream;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;
import org.mockito.invocation.InvocationOnMock;
import org.mockito.stubbing.Answer;

import com.google.common.collect.Maps;

public class UploadCleanserTest {

    @Rule
    public TemporaryFolder root = new TemporaryFolder();
    private FileBuilder fileBuilder;
    private File FILE_IN;
    private File FILE_OUT_EXPECTED;
    private File actualOut;

    private UniqueFileCreator ufc;
    private UploadCleanser sut;

    @Before
    public void setup() throws IOException {
        FILE_IN = new File(root.getRoot(), "in.zip");
        FILE_OUT_EXPECTED = new File(root.getRoot(), "out.zip");

        ufc = mock(UniqueFileCreator.class);
        when(ufc.createNextUniqueFile()).then(new Answer<File>() {
            @Override
            public File answer(InvocationOnMock invocation) throws Throwable {
                FILE_OUT_EXPECTED.createNewFile();
                return FILE_OUT_EXPECTED;
            }
        });
        sut = new UploadCleanser(ufc);
    }

    @Test
    public void fileNameIsRequestedFromUFC() throws IOException {
        givenAFile().with("0.json", json(123));
        purify();
        verify(ufc).createNextUniqueFile();
        assertEquals(FILE_OUT_EXPECTED, actualOut);
    }

    @Test
    public void nonZipFile() throws IOException {
        
        try {
            FileUtils.writeStringToFile(FILE_IN, "XYZ");
            sut.purify(FILE_IN);
            
            fail();
        } catch (KaVEException e) {
            assertEquals(UploadCleanser.NO_ZIP, e.getMessage());
            assertFalse(FILE_IN.exists());
        }
    }

    @Test
    public void emptyFile() {
        givenAFile();
        try {
            purify();
            fail();
        } catch (KaVEException e) {
            assertEquals(UploadCleanser.EMPTY_FILE, e.getMessage());
            assertFalse(FILE_IN.exists());
        }
    }

    @Test
    public void binaryContent() {
        givenAFile().with("a.bin", new byte[] { 127, 13, 45, 114, 3, 72, 96 });
        try {
            purify();
            fail();
        } catch (KaVEException e) {
            assertEquals(UploadCleanser.EMPTY_FILE, e.getMessage());
            assertFalse(FILE_IN.exists());
        }
    }

    @Test
    public void plainTextContent() {
        givenAFile().with("b.txt", "some plain text");
        try {
            purify();
            fail();
        } catch (KaVEException e) {
            assertEquals(UploadCleanser.EMPTY_FILE, e.getMessage());
            assertFalse(FILE_IN.exists());
        }
    }

    @Test
    public void jsonContent() {
        givenAFile().with("0.json", json(123));

        purify();

        assertNumberOfFiles(1);
        assertContents("0.json", json(123));
    }

    @Test
    public void prettyPrintedJsonContentIsFlattened() {
        givenAFile().with("0.json", jsonFancy(123));

        purify();

        assertNumberOfFiles(1);
        assertContents("0.json", jsonFancyFlattened(123));
    }

    @Test
    public void parseableJsonFilesAreRenamed() {
        givenAFile().with("1.txt", json(0));

        purify();

        assertNumberOfFiles(1);
        assertContents("0.json", json(0));
    }

    @Test
    public void emptyFilesAreIgnored() {
        givenAFile().with("0.json", "").with("1.json", json(1));

        purify();

        assertNumberOfFiles(1);
        assertContents("0.json", json(1));
    }

    @Test
    public void existingnumberingIsIgnored() {
        givenAFile().with("1.json", json(0)).with("0.json", json(1));

        purify();

        assertNumberOfFiles(2);
        assertContents("0.json", json(0));
        assertContents("1.json", json(1));
    }

    @Test
    public void subfoldersAreInlined() {
        givenAFile().with("0.json", json(0)).with("sub/1.json", json(1)).with("2.json", json(2))
                .with("sub/3.json", json(3));

        purify();

        assertNumberOfFiles(4);
        assertContents("0.json", json(0));
        assertContents("1.json", json(1));
        assertContents("2.json", json(2));
        assertContents("3.json", json(3));
    }

    @Test
    public void unparsableFilesAreFiltered() {
        givenAFile().with("0.json", json(0)).with("1.txt", new byte[] { 13, 14, 19, 120 }).with("2.json", json(2));

        purify();

        assertNumberOfFiles(2);
        assertContents("0.json", json(0));
        assertContents("1.json", json(2));
    }

    private static String json(int num) {
        return "{\"num\":" + num + "}";
    }

    private static String jsonFancy(int num) {
        return "{\n\t\"num\":" + num + "\n}";
    }

    private static String jsonFancyFlattened(int num) {
        return "{\"num\":" + num + "}";
    }

    private void purify() {
        try {
            fileBuilder.create();
            actualOut = sut.purify(FILE_IN);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }

    private void assertNumberOfFiles(int expectedNumOfFiles) {
        Set<String> actualNames = readZipFile(actualOut).keySet();
        assertEquals(expectedNumOfFiles, actualNames.size());
        for (int i = 0; i < expectedNumOfFiles; i++) {
            String expectedName = i + ".json";
            assertTrue(actualNames.contains(expectedName));
        }
    }

    private void assertContents(String fileName, String expectedContent) {
        String actualContent = readZipFile(actualOut).get(fileName);
        assertNotNull(actualContent);
        assertEquals(expectedContent, actualContent);
    }

    public static Map<String, String> readZipFile(File f) {
        Map<String, String> fileContent = Maps.newLinkedHashMap();
        ZipFile zipFile = null;
        InputStream entryStream = null;
        try {
            zipFile = new ZipFile(f);
            Enumeration<? extends ZipEntry> entries = zipFile.entries();
            while (entries.hasMoreElements()) {
                ZipEntry ze = entries.nextElement();
                String fileName = ze.getName();
                entryStream = zipFile.getInputStream(ze);
                String content = IOUtils.toString(entryStream);
                entryStream.close();
                fileContent.put(fileName, content);
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        } finally {
            closeQuietly(zipFile);
        }
        return fileContent;
    }

    private static void closeQuietly(Closeable c) {
        try {
            if (c != null) {
                c.close();
            }
        } catch (Exception e) {
            // ignore
        }
    }

    public FileBuilder givenAFile() {
        fileBuilder = new FileBuilder(FILE_IN);
        return fileBuilder;
    }

    public static class FileBuilder {

        private File file;
        Map<String, String> contents = Maps.newLinkedHashMap();

        public FileBuilder(File file) {
            this.file = file;
        }

        public FileBuilder with(String name, String content) {
            contents.put(name, content);
            return this;
        }

        public FileBuilder with(String name, byte[] content) {
            contents.put(name, new String(content, Charset.forName("UTF-8")));
            return this;
        }

        public File create() {
            try {
                final ZipOutputStream out = new ZipOutputStream(new FileOutputStream(file));

                for (Entry<String, String> entry : contents.entrySet()) {
                    out.putNextEntry(new ZipEntry(entry.getKey()));
                    byte[] byteContent = entry.getValue().getBytes(Charset.forName("UTF-8"));
                    out.write(byteContent, 0, byteContent.length);
                    out.closeEntry();
                }
                out.close();

                return file;
            } catch (Exception e) {
                throw new RuntimeException(e);
            }
        }
    }
}