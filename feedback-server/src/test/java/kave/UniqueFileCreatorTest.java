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

import java.io.File;
import java.io.IOException;

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;

public class UniqueFileCreatorTest {

    @Rule
    public TemporaryFolder root = new TemporaryFolder();

    private UniqueFileCreator sut;

    @Before
    public void setup() {
        sut = new UniqueFileCreator(root.getRoot(), "zip");
    }

    @Test(expected = KaVEException.class)
    public void rootMustExistBeforehands() throws IOException {
        sut = new UniqueFileCreator(new File(root.getRoot(), "doesNotExist"), "zip");
    }

    @Test(expected = KaVEException.class)
    public void rootMustBeFolder() throws IOException {
        File f = root.newFile("isFile");
        sut = new UniqueFileCreator(f, "zip");
    }

    @Test(expected = KaVEException.class)
    public void extensionDoesNotStartWithDot() throws IOException {
        sut = new UniqueFileCreator(root.getRoot(), ".zip");
    }

    @Test
    public void filesStartByZero() throws IOException {
        assertCorrectNaming(0, "zip");
    }

    @Test
    public void extensionIsRespected() throws IOException {
        sut = new UniqueFileCreator(root.getRoot(), "txt");
        assertCorrectNaming(0, "txt");
    }

    @Test
    public void multipleInvocationsIncreaseCounter() throws IOException {
        assertCorrectNaming(0, "zip");
        assertCorrectNaming(1, "zip");
        assertCorrectNaming(2, "zip");
    }

    @Test
    public void preexistingFilesAreNotAnIssue() throws IOException {
        root.newFile("0.zip");
        setup();
        assertCorrectNaming(1, "zip");
    }

    @Test
    public void collidingFileNamesAreDetected() throws IOException {
        root.newFile("1.zip");
        setup();
        assertCorrectNaming(2, "zip");
    }

    @Test
    public void numberingOnlyDependsOnNumberOfFiles() throws IOException {
        root.newFile("1.txt");
        setup();
        assertCorrectNaming(1, "zip");
    }

    private void assertCorrectNaming(int num, String extension) throws IOException {
        File actual = sut.createNextUniqueFile();
        File expected = new File(root.getRoot(), num + "." + extension);
        assertEquals(expected, actual);
    }
}