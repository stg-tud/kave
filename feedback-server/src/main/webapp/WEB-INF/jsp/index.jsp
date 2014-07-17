<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<script type="text/javascript" src="js/jquery-2.0.3.js"></script>
<script type="text/javascript" src="js/require-2.1.10.js"
	data-main="js/index.js"></script>
<script type="text/javascript" src="js/growl/jquery.growl.js"></script>
<link rel="stylesheet" type="text/css" href="css/growl/jquery.growl.css" />
<link rel="stylesheet" type="text/css" href="css/feedback.css" />
</head>
<body>
	<noscript>WARNUNG: Der Upload ben&ouml;tigt JavaScript.</noscript>

	<div id="content">
		<h1>Hochladen des Feedbacks</h1>

		<p>Bitte w&auml;hlen Sie die Feedback-Datei aus, die Sie aus dem
			Feedback-Manager exportiert haben und best&auml;tigen Sie,
			dass Sie den Datenschutzhinweis gelesen haben.</p>

		<form id="file-upload-form" enctype="multipart/form-data">
			<input type="file" id="file" name="file" /><br />
			<input type="checkbox" id="confirm" />
			<div id="hint">
				Mir ist bewusst, dass mein Feedback durch das Best&auml;tigen des
				"Hochladen"-Knopfs auf dem Server hinterlegt wird. Die Daten haben
				keinerlei Personenbezug, daher habe ich danach keinen Zugriff mehr
				auf die Daten.<br />
				<br /> Das eingereichte Feedback kann nicht zu mir zur&uuml;ckverfolgt
				werden.
			</div>
			<button id="submit-upload" disabled="disabled">Hochladen</button>
		</form>
	</div>
</body>
</html>
