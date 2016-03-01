<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<script type="text/javascript" src="js/jquery-2.0.3.js"></script>
<script type="text/javascript" src="js/require-2.1.10.js"
	data-main="js/index.js"></script>
<script type="text/javascript" src="js/growl/jquery.growl.js"></script>
<link rel="stylesheet" type="text/css" href="css/reset.css" />
<link rel="stylesheet" type="text/css" href="css/growl/jquery.growl.css" />
<link rel="stylesheet" type="text/css" href="css/feedback.css" />
<title>Uploading Captured IDE Interactions</title>
</head>
<body>
	<noscript>
		<b>WARNUNG:</b> Der Upload ben&ouml;tigt JavaScript.
	</noscript>

	<div id="content">
		<div id="header">
			<a href="http://kave.cc" title="KaVE-Projekt-Webseite"> <img
				alt="KaVE-Projekt" src="images/kave_logo_with_title.png" />
			</a>
		</div>
	
		<p>
			You are now ready to hand in the captured interactions with your IDE! What data does this contain?
                In short, we monitor how you use Visual Studio and capture some information to explain your actions.
                For example, when you trigger IntelliSense, we capture information about the surrounding source code and your selected proposal.
                Note that <span class="it">we do not capture any personal information!</span>
		</p>
		
		<p>We are super happy that you want to submit your interactions, but we need to make sure that you fully understand the following:</p>

		 <fieldset>
			<legend>Disclaimer</legend>
		
            <ul>
                <li>We provide a <a href="http://www.kave.cc/documentation/event-generation">detailed explanation</a> of everything we capture and you can review the contents of the file you are about to submit.
                	It is your responsibility to verify that the interactions you submit do not contain anything you don't want to share.
                	We cannot identify your interactions after the submission and so you have no way of revoking any of it.
                 </li>
                
				<li>We will group your subsequent submissions, e.g., by matching your profile id.</li>

                <li>We plan to include your interactions in a public dataset that is part of scientific publications.</li>

                <li> We do not include any personal information in the interactions, but we cannot guarantee that third parties will not
                        try to match the data with other publicly available information.</li>

                <li class="important">Some interactions contain a simplified version of the source code you were working on.
                            Make sure that you did not work on source code that must stay non-disclosed.</li>
            </ul>
        </fieldset>
		
	 	<h1>Submitting Captured IDE Interactions</h1>
    
       <p>You are almost done now! Select the file for submission and confirm the disclaimer:</p>


		<img src="images/spinner.gif" class="spinner" />
		<form id="file-upload-form" enctype="multipart/form-data">
			<input type="file" id="file" name="file" /><br />
			<input type="checkbox" id="confirm" /><span> I carefully read the disclaimer, fully understand its implications, and confirm my agreement to it.</span><br />
			<button id="submit-upload" disabled="disabled">Submit</button>
		</form>


		<div id="footer">

		<p>If you have questions about any of this, please <a href="http://www.kave.cc/community">directly get in contact with us</a>.
                We would love to hear from you and help with anything!
                You can also use our <a href="http://www.kave.cc/feedback">anonymous feedback form</a>.</p>

		<dl class="cf space">
			<dt>Project lead / legal contact:</dt>
			<dd><a href="http://www.stg.tu-darmstadt.de/staff/sebastian_proksch/">Sebastian Proksch</a></dd>
			<dt>More Information:</dt>
			<dd><a href="http://www.kave.cc">KaVE Project Website</a></dd>
		</dl>
		</div>
	</div>
</body>
</html>
