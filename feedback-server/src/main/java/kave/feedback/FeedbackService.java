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
	private File root;

	@Inject
	public FeedbackService(File root) throws IOException {
		this.root = root;
		enforceOutputDirectory();
	}

	private void enforceOutputDirectory() throws IOException {
		if (!getOutputFolder().exists()) {
			FileUtils.forceMkdir(getOutputFolder());
		}
	}

	private File getOutputFolder() {
		return new File(root, "data");
	}

	@GET
	public Viewable index() {
		return new Viewable("/index.jsp");
	}

	@POST
	@Path("upload")
	@Consumes(MediaType.MULTIPART_FORM_DATA)
	@Produces(MediaType.APPLICATION_JSON)
	public Result<Void> upload(@FormDataParam("file") InputStream fileInputStream,
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
		File outputFolder = getOutputFolder();
		int lastExistingFileIndex = 0;
		for (String fileName : outputFolder.list()) {
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
		return new File(outputFolder, String.format("%05d",
				(lastExistingFileIndex + 1)) + "." + fileExtension);
	}

}