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

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;

import com.google.inject.Inject;
import com.sun.jersey.api.view.Viewable;
import com.sun.jersey.multipart.BodyPart;
import com.sun.jersey.multipart.BodyPartEntity;
import com.sun.jersey.multipart.MultiPart;

@Path("/")
public class FeedbackService {

    public static final String NO_SINGLE_UPLOAD = "You can only upload one file at a time!";
    public static final String UPLOAD_FAILED = "Upload failed!";

    private final File dataFolder;
    private final File tmpFolder;
    private final UploadCleanser cleanser;
    private final UniqueFileCreator tmpufc;
    private final UniqueFileCreator dataUfc;

    @Inject
    public FeedbackService(File dataFolder, File tmpFolder, UploadCleanser cleanser, UniqueFileCreator tmpufc,
            UniqueFileCreator dataUfc) throws IOException {
        this.dataFolder = dataFolder;
        this.tmpFolder = tmpFolder;
        this.cleanser = cleanser;
        this.tmpufc = tmpufc;
        this.dataUfc = dataUfc;
        enforceFolders();
    }

    private void enforceFolders() throws IOException {
        if (!dataFolder.exists()) {
            FileUtils.forceMkdir(dataFolder);
        }
        if (!tmpFolder.exists()) {
            FileUtils.forceMkdir(tmpFolder);
        }
    }

    @GET
    @Produces(MediaType.TEXT_HTML + "; charset=utf-8")
    public Viewable index() {
        return new Viewable("/index.jsp");
    }

    @POST
    @Consumes(MediaType.MULTIPART_FORM_DATA)
    @Produces(MediaType.APPLICATION_JSON)
    public Result upload(MultiPart data) {
        try {
            if (!isSingleFileUpload(data)) {
                return Result.fail(NO_SINGLE_UPLOAD);
            }

            File tmpFile = storeTmp(data);
            File purifiedFile = cleanser.purify(tmpFile);

            moveToData(purifiedFile);

            return Result.ok();
        } catch (KaVEException e) {
            return Result.fail(e.getMessage());
        } catch (Exception e) {
            return Result.fail(UPLOAD_FAILED);
        }
    }

    private boolean isSingleFileUpload(MultiPart data) {
        int numParts = data.getBodyParts().size();
        return numParts == 1;
    }

    private File storeTmp(MultiPart data) throws IOException {
        FileOutputStream outputStream = null;
        try {
            BodyPart bp = data.getBodyParts().get(0);
            BodyPartEntity bpe = (BodyPartEntity) bp.getEntity();

            File f = tmpufc.createNextUniqueFile();
            outputStream = new FileOutputStream(f);
            IOUtils.copy(bpe.getInputStream(), outputStream);

            return f;
        } finally {
            IOUtils.closeQuietly(outputStream);
        }
    }

    private void moveToData(File tmpFile) throws IOException {
        File dataFile = dataUfc.createNextUniqueFile();
        FileUtils.moveFile(tmpFile, dataFile);
    }
}