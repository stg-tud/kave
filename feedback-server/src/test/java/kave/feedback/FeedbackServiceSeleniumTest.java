package kave.feedback;
import static kave.feedback.TestHelper.*;
import static org.junit.Assert.*;

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

import static org.openqa.selenium.support.ui.ExpectedConditions.*;

public class FeedbackServiceSeleniumTest {
	
	private static final By GrowlNoticeLocator = By.className("growl-notice");
	private static final By GrowlErrorLocator = By.className("growl-error");
	private static File OutputDirectory = new File("data/data");
	
	@Rule
	public TemporaryFolder tempFolder = new TemporaryFolder();
	
	private boolean isJavascriptEnabled = true;
	private FirefoxDriver driver;
	private FirefoxProfile profile;
	
	@Before @After
	public void clearOutputDirectory() throws IOException {
		FileUtils.deleteDirectory(OutputDirectory);
	}
	
	@After
	public void closeBrowser() {
		driver.close();
		driver.quit();
	}
	
	@Test
	public void shouldBeDisabledIfJavascriptIsDisabled(){
		isJavascriptEnabled = false;
		givenPageIsLoaded();
		
		WebElement submitButton = driver.findElementByXPath("//button");
		assertFalse(submitButton.isEnabled());
	}
	
	@Test
    public void shouldSucceedIfInputIsCorrect() throws IOException {
		givenPageIsLoaded();
		
    	File fileToUpload = createRandomFile(tempFolder, "my-test-file.zip");
    	
    	whenFileIsSelected(fileToUpload);
		whenConfirmationIsSelected();
		whenFormIsSubmitted();
		
		thenResponseIs(GrowlNoticeLocator, "Die Datei wurde erfolgreich hochgeladen.");
		assertDirectoryContainsFile(OutputDirectory, "00001.zip", fileToUpload);
    }
	
	@Test
	public void shouldFailIfConfirmationIsNotSelected() throws IOException {
		givenPageIsLoaded();
		
		File fileToUpload = createRandomFile(tempFolder, "random-test-file.zip");
		
    	whenFileIsSelected(fileToUpload);
		whenFormIsSubmitted();
		
		thenResponseIs(GrowlErrorLocator, "Bitte stimmen Sie der Einverständniserklärung zu, bevor Sie die Datei hochladen.");
	}
	
	@Test
	public void shouldFailIfNothingSelected() throws IOException {
		givenPageIsLoaded();
		
		whenFormIsSubmitted();
		
		thenResponseIs(GrowlErrorLocator, "Bitte stimmen Sie der Einverständniserklärung zu, bevor Sie die Datei hochladen.");
	}
	
	@Test
	public void shouldFailIfNoFileSelected() throws IOException {
		givenPageIsLoaded();
		
		whenConfirmationIsSelected();
		whenFormIsSubmitted();
		
		thenResponseIs(GrowlErrorLocator, "Es wurde keine Datei zum Hochladen ausgewählt. Bitte wählen Sie eine Datei.");
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
