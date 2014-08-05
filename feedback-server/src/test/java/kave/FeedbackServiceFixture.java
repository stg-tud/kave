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

import static org.apache.commons.io.FileUtils.writeByteArrayToFile;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.nio.charset.Charset;
import java.util.List;
import java.util.Random;
import java.util.zip.ZipEntry;
import java.util.zip.ZipException;
import java.util.zip.ZipOutputStream;

import javax.ws.rs.core.MediaType;

import com.google.common.collect.Lists;
import com.sun.jersey.multipart.BodyPart;
import com.sun.jersey.multipart.BodyPartEntity;
import com.sun.jersey.multipart.MultiPart;

public class FeedbackServiceFixture {

    private static final MediaType APPLICATION_ZIP = new MediaType("application", "zip");

    private static Random random = new Random();

    private File dataDir;
    private File tmpDir;

    private File rndFile;
    private File zipFile;

    private File emptyZipFile;

    public FeedbackServiceFixture(File root) throws IOException {
        dataDir = new File(root, "data");
        tmpDir = new File(root, "tmp");

        rndFile = new File(root, "rnd.txt");
        writeByteArrayToFile(rndFile, createRandomByteArray());

        zipFile = new File(root, "zip.zip");
        writeZipFile(zipFile);

        emptyZipFile = new File(root, "emptyZip.zip");
        writeEmptyZipFile(emptyZipFile);
    }

    private void writeEmptyZipFile(File f) throws IOException {
        ZipOutputStream out = new ZipOutputStream(new FileOutputStream(f));
        out.close();
    }

    private void writeZipFile(File f) throws ZipException, IOException {
        ZipOutputStream out = new ZipOutputStream(new FileOutputStream(f));
        out.putNextEntry(new ZipEntry("0.json"));

        out.write("{\"asd\":4}".getBytes(Charset.forName("UTF-8")));
        out.closeEntry();

        out.close();
    }

    private static byte[] createRandomByteArray() {
        int arraySize = random.nextInt(64);
        byte[] content = new byte[arraySize];
        random.nextBytes(content);
        return content;
    }

    public File getDataFolder() {
        return dataDir;
    }

    public File getTempFolder() {
        return tmpDir;
    }

    public MultiPart createMultiFileUpload() throws FileNotFoundException {
        List<BodyPart> parts = Lists.newLinkedList();
        parts.add(createRandomBodyPart("a.txt"));
        parts.add(createRandomBodyPart("b.txt"));
        MultiPart data = mock(MultiPart.class);
        when(data.getBodyParts()).thenReturn(parts);
        return data;
    }

    public MultiPart createRandomFileUpload() throws FileNotFoundException {
        List<BodyPart> parts = Lists.newLinkedList();
        parts.add(createRandomBodyPart("a.txt"));
        MultiPart data = mock(MultiPart.class);
        when(data.getBodyParts()).thenReturn(parts);
        return data;
    }

    public MultiPart createZipFileUpload() throws FileNotFoundException {
        List<BodyPart> parts = Lists.newLinkedList();
        parts.add(createZipBodyPart("a.zip"));
        MultiPart data = mock(MultiPart.class);
        when(data.getBodyParts()).thenReturn(parts);
        return data;
    }

    private BodyPart createZipBodyPart(String string) throws FileNotFoundException {
        BodyPartEntity bpe = mock(BodyPartEntity.class);
        when(bpe.getInputStream()).thenReturn(new FileInputStream(zipFile));
        BodyPart bp = mock(BodyPart.class);
        when(bp.getMediaType()).thenReturn(APPLICATION_ZIP);
        when(bp.getEntity()).thenReturn(bpe);
        return bp;
    }

    private BodyPart createRandomBodyPart(String s) throws FileNotFoundException {
        BodyPartEntity bpe = mock(BodyPartEntity.class);
        when(bpe.getInputStream()).thenReturn(new FileInputStream(rndFile));

        BodyPart bp = mock(BodyPart.class);
        when(bp.getMediaType()).thenReturn(MediaType.TEXT_PLAIN_TYPE);
        when(bp.getEntity()).thenReturn(bpe);
        return bp;
    }

    public File getRandomFile() {
        return rndFile;
    }

    public File getZipFile() {
        return zipFile;
    }

    public void createFileInDataFolder() throws IOException {
        String fileName = dataDir.list().length + ".zip";
        createFileInDataFolder(fileName);
    }

    public void createFileInDataFolder(String fileName) throws IOException {
        new File(dataDir, fileName).createNewFile();
    }

    public MultiPart createFailingUpload() throws FileNotFoundException {
        // failing file
        BodyPartEntity bpe = mock(BodyPartEntity.class);
        when(bpe.getInputStream()).thenThrow(new RuntimeException());
        BodyPart bp = mock(BodyPart.class);
        when(bp.getMediaType()).thenReturn(APPLICATION_ZIP);
        when(bp.getEntity()).thenReturn(bpe);

        // body part
        List<BodyPart> parts = Lists.newLinkedList();
        parts.add(bp);
        MultiPart data = mock(MultiPart.class);
        when(data.getBodyParts()).thenReturn(parts);
        return data;
    }

    public File getEmptyZipFile() {
        return emptyZipFile;
    }
}