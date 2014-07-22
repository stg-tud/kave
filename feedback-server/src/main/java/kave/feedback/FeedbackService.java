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
        FileOutputStream outputStream = null;
    	try {
            String fileName = contentDisposition.getFileName();
            File freshOutputFile = getNextOutputFile();
            outputStream = new FileOutputStream(freshOutputFile);
            IOUtils.copy(fileInputStream, outputStream);
            return Result.ok();
        } catch (Exception e) {
            return Result.fail(e);
        }
    	finally {
    		IOUtils.closeQuietly(outputStream);
    	}
    }

    private synchronized File getNextOutputFile() throws IOException {
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
        File newFile = new File(dataFolder, String.format("%05d", (lastExistingFileIndex + 1)) + ".zip");

        // ensure the file is a newly created file 
        if (! newFile.createNewFile()) {
        	throw new IOException("cannot create new file " + newFile.getName());
        }
        return newFile;
    }

}