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

import java.io.Closeable;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Enumeration;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipException;
import java.util.zip.ZipFile;
import java.util.zip.ZipOutputStream;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;
import org.codehaus.jackson.JsonGenerator;
import org.codehaus.jackson.map.ObjectMapper;

public class UploadCleanser {

    public static final String EMPTY_FILE = "File is empty or does not contain valid interactions.";
    public static final String NO_ZIP = "No valid .zip file provided";

    private UniqueFileCreator ufc;
    private ObjectMapper mapper;

    public UploadCleanser(UniqueFileCreator ufc) {
        this.ufc = ufc;
        mapper = new ObjectMapper();
        mapper.configure(JsonGenerator.Feature.AUTO_CLOSE_TARGET, false);
    }

    public File purify(File in) throws IOException {
        File out = ufc.createNextUniqueFile();
        ZipFile zfin = null;

        try {
            zfin = new ZipFile(in);
            cloneTo(zfin, out);
        } catch (ZipException e) {
            FileUtils.deleteQuietly(in);
            FileUtils.deleteQuietly(out);
            throw new KaVEException(NO_ZIP);
        } catch (KaVEException ke) {
            FileUtils.deleteQuietly(in);
            FileUtils.deleteQuietly(out);
            throw ke;
        } finally {
            closeQuietly(zfin);
        }

        return out;
    }

    private void cloneTo(ZipFile zfin, File out) throws FileNotFoundException, IOException {
        ZipOutputStream zfout = null;
        try {
            int num = 0;
            zfout = new ZipOutputStream(new FileOutputStream(out));

            Enumeration<? extends ZipEntry> entries = zfin.entries();
            while (entries.hasMoreElements()) {
                ZipEntry entry = entries.nextElement();
                Map<String, Object> content = readEntry(zfin, entry);

                if (content != null) {
                    String fileName = (num++) + ".json";
                    putEntry(content, zfout, fileName);
                }
            }
            if (num == 0) {
                throw new KaVEException(EMPTY_FILE);
            }
        } finally {
            IOUtils.closeQuietly(zfout);
        }
    }

    @SuppressWarnings("unchecked")
    private Map<String, Object> readEntry(ZipFile zfin, ZipEntry entry) throws IOException {
        InputStream zein = null;
        Map<String, Object> content = null;
        try {
            zein = zfin.getInputStream(entry);
            content = mapper.readValue(zein, Map.class);
        } catch (Exception e) {
            // just ignore file
        } finally {
            IOUtils.closeQuietly(zein);
        }
        return content;
    }

    private void putEntry(Map<String, Object> content, ZipOutputStream zfout, String fileName) throws IOException {
        zfout.putNextEntry(new ZipEntry(fileName));
        mapper.writeValue(zfout, content);
        zfout.closeEntry();
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
}