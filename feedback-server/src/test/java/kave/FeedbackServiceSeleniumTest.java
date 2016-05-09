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

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertFalse;
import static org.openqa.selenium.support.ui.ExpectedConditions.presenceOfElementLocated;

import java.io.File;
import java.io.IOException;

import org.apache.commons.io.FileUtils;
import org.junit.After;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.TemporaryFolder;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.firefox.FirefoxDriver;
import org.openqa.selenium.firefox.FirefoxProfile;
import org.openqa.selenium.support.ui.WebDriverWait;

/*
 * make sure you run this test via Maven. If you would like to run it from
 * within Eclipse (e.g., to debug it), you have to manually start the server
 */
public class FeedbackServiceSeleniumTest {

    private static final By growlNoticeLocator = By.className("growl-notice");
    private static final By growlErrorLocator = By.className("growl-error");

    private static File dataDir = new File("data");
    private static File tmpDir = new File("tmp");

    @Rule
    public TemporaryFolder tempFolder = new TemporaryFolder();

    private boolean isJavascriptEnabled = true;
    private FirefoxDriver driver;
    private FirefoxProfile profile;
    private FeedbackServiceFixture fix;

    @Before
    @After
    public void clearOutputDirectory() throws IOException {
        fix = new FeedbackServiceFixture(tempFolder.getRoot());
        FileUtils.deleteDirectory(dataDir);
        FileUtils.deleteDirectory(tmpDir);
    }

    @After
    public void closeBrowser() {
        driver.close();
        driver.quit();
    }

    @Test
    public void shouldBeDisabledIfJavascriptIsDisabled() {
        isJavascriptEnabled = false;
        givenPageIsLoaded();

        WebElement submitButton = driver.findElementByXPath("//button");
        assertFalse(submitButton.isEnabled());
    }

    @Test
    public void shouldSucceedIfInputIsCorrect() throws IOException {
        givenPageIsLoaded();

        File fileToUpload = fix.getZipFile();

        whenFileIsSelected(fileToUpload);
        whenConfirmationIsSelected();
        whenFormIsSubmitted();

        thenResponseIs(growlNoticeLocator, "File upload was successful.");
        File fileName = findUploadedFileName();
        FeedbackServiceTest.assertDirectoryContainsZipFile(fileName.getParentFile(), fileName.getName(), fileToUpload);
    }

    private File findUploadedFileName() {
        String[] dates = dataDir.list();
        File subFolder = new File(dataDir, dates[0]);
        String[] zips = subFolder.list();
        return new File(subFolder, zips[0]);
    }

    @Test
    public void shouldFailIfInputIsNotAZip() throws IOException {
        givenPageIsLoaded();

        File fileToUpload = fix.getRandomFile();

        whenFileIsSelected(fileToUpload);
        whenConfirmationIsSelected();
        whenFormIsSubmitted();

        thenResponseIs(growlErrorLocator, UploadCleanser.NO_ZIP);
    }

    @Test
    public void shouldFailIfInputIsEmptyZip() throws IOException {
        givenPageIsLoaded();

        File fileToUpload = fix.getEmptyZipFile();

        whenFileIsSelected(fileToUpload);
        whenConfirmationIsSelected();
        whenFormIsSubmitted();

        thenResponseIs(growlErrorLocator, UploadCleanser.EMPTY_FILE);
    }

    @Test
    public void shouldFailIfConfirmationIsNotSelected() throws IOException {
        givenPageIsLoaded();

        File fileToUpload = fix.getRandomFile();

        whenFileIsSelected(fileToUpload);
        whenFormIsSubmitted();

        thenResponseIs(growlErrorLocator, "Please confirm the disclaimer before submitting a file.");
    }

    @Test
    public void shouldFailIfNothingSelected() throws IOException {
        givenPageIsLoaded();

        whenFormIsSubmitted();

        thenResponseIs(growlErrorLocator, "Please confirm the disclaimer before submitting a file.");
    }

    @Test
    public void shouldFailIfNoFileSelected() throws IOException {
        givenPageIsLoaded();

        whenConfirmationIsSelected();
        whenFormIsSubmitted();

        thenResponseIs(growlErrorLocator, "No file was selected.");
    }

    private void givenPageIsLoaded() {
        profile = new FirefoxProfile();
        profile.setPreference("javascript.enabled", isJavascriptEnabled);
        driver = new FirefoxDriver(profile);
        driver.get("http://localhost:8080");
    }

    private void whenConfirmationIsSelected() {
        WebElement confirmationCheckbox = driver.findElementByXPath("//input[@type='checkbox']");
        confirmationCheckbox.click();
    }

    private void whenFileIsSelected(File fileToUpload) {
        WebElement fileInput = driver.findElementByXPath("//input[@type='file']");
        fileInput.sendKeys(fileToUpload.getAbsolutePath());
    }

    private void whenFormIsSubmitted() {
        WebElement submitButton = driver.findElementByXPath("//button");
        submitButton.click();
    }

    private void thenResponseIs(By growlLocator, String message) {
        new WebDriverWait(driver, 5).until(presenceOfElementLocated(growlLocator));
        WebElement growl = driver.findElement(growlLocator);
        WebElement title = growl.findElement(By.className("growl-title"));
        assertEquals(message, title.getText());
    }
}
