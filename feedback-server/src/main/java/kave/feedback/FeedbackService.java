/**
 * Copyright (c) 2011-2014 Darmstadt University of Technology.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 */
package kave.feedback;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import javax.ws.rs.Consumes;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

import kave.Result;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.io.IOUtils;

import com.google.inject.Inject;
import com.sun.jersey.api.view.Viewable;
import com.sun.jersey.core.header.FormDataContentDisposition;
import com.sun.jersey.multipart.FormDataParam;

@Path("/")
public class FeedbackService {
    private File dataFolder;

    @Inject
    public FeedbackService(File dataFolder) throws IOException {
        this.dataFolder = dataFolder;
        enforceOutputDirectory();
    }

    private void enforceOutputDirectory() throws IOException {
        if (!dataFolder.exists()) {
            FileUtils.forceMkdir(dataFolder);
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
    public Result upload(@FormDataParam("file") InputStream fileInputStream,
            @FormDataParam("file") FormDataContentDisposition contentDisposition) {
        try {
            String fileName = contentDisposition.getFileName();
            String extension = FilenameUtils.getExtension(fileName);
            File freshOutputFile = getFreshOutputFile(extension);
            IOUtils.copy(fileInputStream, new FileOutputStream(freshOutputFile));
            return Result.ok();
        } catch (Exception e) {
            return Result.fail(e);
        }
    }

    private File getFreshOutputFile(String fileExtension) {
        int lastExistingFileIndex = 0;
        for (String fileName : dataFolder.list()) {
            try {
                String baseName = FilenameUtils.getBaseName(fileName);
                int currentFileIndex = Integer.parseInt(baseName);
                if (currentFileIndex > lastExistingFileIndex) {
                    lastExistingFileIndex = currentFileIndex;
                }
            } catch (NumberFormatException nfe) {
                // just ignore the file, because there will be no collision with
                // our number schema
            }
        }
        return new File(dataFolder, String.format("%05d", (lastExistingFileIndex + 1)) + "." + fileExtension);
    }

}