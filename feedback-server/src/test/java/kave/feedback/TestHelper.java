/**
 * Copyright (c) 2010, 2011 Darmstadt University of Technology.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 */
package kave.feedback;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

import java.io.File;
import java.io.IOException;
import java.util.Random;

import org.apache.commons.io.FileUtils;
import org.junit.rules.TemporaryFolder;

public abstract class TestHelper {

	private static Random random = new Random();

	public static File createRandomFile(TemporaryFolder temporaryFolder,
			String fileName) throws IOException {
		File temporaryFile = temporaryFolder.newFile(fileName);
		byte[] content = createRandomByteArray();
		FileUtils.writeByteArrayToFile(temporaryFile, content);
		return temporaryFile;
	}

	private static byte[] createRandomByteArray() {
		int arraySize = random.nextInt(64);
		byte[] content = new byte[arraySize];
		random.nextBytes(content);
		return content;
	}

	public static void assertDirectoryContainsFile(File directory,
			final String expectedFileName, File expectedFileContent)
			throws IOException {
		File actualFile = new File(directory, expectedFileName);
		assertTrue(actualFile.exists());
		long expectedFileHash = FileUtils.checksumCRC32(expectedFileContent);
		long actualFileHash = FileUtils.checksumCRC32(actualFile);
		assertEquals(expectedFileHash, actualFileHash);
	}

}
