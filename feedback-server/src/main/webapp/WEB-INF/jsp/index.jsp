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
	<noscript>
		<b>WARNUNG:</b> Der Upload ben&ouml;tigt JavaScript.
	</noscript>

	<div id="content">
		<div id="header">
			<a href="http://kave.cc" title="KaVE-Projekt-Webseite"> <img
				alt="KaVE-Projekt" src="images/kave_logo_with_title.png" />
			</a>
		</div>
		<h1>Hochladen des Feedbacks</h1>

		<p>Diese Seite dient zum Hochladen von Benutzungsfeedback, das mit
			dem &ldquo;KaVE Feedback Generator&rdquo; erzeugt und exportiert
			wurde. Bitte w&auml;hlen Sie eine Feedback-Exportdatei aus,
			best&auml;tigen Sie, dass Sie den Datenschutzhinweis gelesen haben
			und klicken Sie auf &ldquo;Hochladen&rdquo;.</p>

		<form id="file-upload-form" enctype="multipart/form-data">
			<input type="file" id="file" name="file" /><br /> <input
				type="checkbox" id="confirm" />
			<div id="hint">
				Mir ist bewusst, dass mein Feedback nach dem Klick auf "Hochladen"
				auf dem Server hinterlegt wird. Das Feedback hat keinerlei
				Personenbezug, weshalb auch ich anschlieﬂend nicht mehr darauf
				zugreifen kann.<br />
				<br />Das eingereichte Feedback kann nicht zu mir
				zur&uuml;ckverfolgt werden.
			</div>
			<button id="submit-upload" disabled="disabled">Hochladen</button>
		</form>
		<hr />
		<dl id="footer">
			<dt>Ansprechpartner bei DATEV:</dt>
			<dd>
				<a href="mailto:fischer@datev.de">Andreas Fischer</a>
			</dd>
			<dt>Informationen zum KaVE-Projekt:</dt>
			<dd>
				<a href="http://kave.cc" title="KaVE-Projekt-Webseite">kave.cc</a>
			</dd>
		</dl>
	</div>
</body>
</html>
