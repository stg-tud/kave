<html>
<head>
<script type="text/javascript" src="js/jquery-2.0.3.js"></script>
<script type="text/javascript" src="js/require-2.1.10.js"
	data-main="js/index.js"></script>
<script type="text/javascript" src="js/growl/jquery.growl.js"></script>
<link rel="stylesheet" type="text/css" href="css/growl/jquery.growl.css" />
</head>
<body>
	<noscript>
	WARNUNG: Der Upload benötigt JavaScript.
	</noscript>

	<form id="file-upload-form" enctype="multipart/form-data">
		<input type="file" id="file" name="file" /><br/>
		<input type="checkbox" id="confirm" /> Ich bin einverstanden<br/>
		<button id="submit-upload" disabled="disabled">Hochladen</button>
	</form>

</body>
</html>
