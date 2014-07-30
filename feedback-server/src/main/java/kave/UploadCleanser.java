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

import java.io.Closeable;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Enumeration;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipOutputStream;

import org.apache.commons.io.IOUtils;
import org.codehaus.jackson.JsonGenerator;
import org.codehaus.jackson.JsonParseException;
import org.codehaus.jackson.map.ObjectMapper;

public class UploadCleanser {

    private UniqueFileCreator ufc;

    public UploadCleanser(UniqueFileCreator ufc) {
        this.ufc = ufc;
    }

    public File purify(File in) throws IOException {
        ObjectMapper mapper = new ObjectMapper();
        mapper.configure(JsonGenerator.Feature.AUTO_CLOSE_TARGET, false);
        
        File out = ufc.createNextUniqueFile();
        int num = 0;

        ZipFile zfin = null;
        ZipOutputStream zfout = null;

        try {
            zfin = new ZipFile(in);
            zfout = new ZipOutputStream(new FileOutputStream(out));

            Enumeration<? extends ZipEntry> entries = zfin.entries();
            while (entries.hasMoreElements()) {
                ZipEntry entry = entries.nextElement();
                String nextFileName = (num++) + ".json";

                InputStream zein = null;
                try {
                    zein = zfin.getInputStream(entry);
                    @SuppressWarnings("unchecked")
                    Map<String, Object> content = mapper.readValue(zein, Map.class);

                    zfout.putNextEntry(new ZipEntry(nextFileName));
                    mapper.writeValue(zfout, content);
                    zfout.closeEntry();
                } catch (JsonParseException jpe) {
                    // just ignore file
                } finally {
                    IOUtils.closeQuietly(zein);
                }

            }
        } finally {
            closeQuietly(zfin);
            IOUtils.closeQuietly(zfout);
        }

        return out;
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