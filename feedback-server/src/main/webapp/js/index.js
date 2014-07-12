require([], function() {
	
	$("#submit-upload").removeAttr("disabled")
	
	$("#submit-upload").click(function(e) {
		e.preventDefault()
		e.stopPropagation()
		
		var cbFail = function(msg) {
			$.growl.error({
				"title":msg,
				"message":""
			})
		}
		
		var confirmation = $("#confirm")[0]
		if (!confirmation.checked) {
			cbFail("Bitte stimmen Sie der Einverständniserklärung zu, bevor Sie die Datei hochladen.")
			return
		}
		
		var fileInput = $("#file")[0]
		var files = fileInput.files
		if (files.length == 0) {
			cbFail("Es wurde keine Datei zum Hochladen ausgewählt. Bitte wählen Sie eine Datei.");
			return
		} else if (files.length > 1) {
			cbFail("Bitte nur eine Datei gleichzeitig auswählen.")
			return
		}
		
		// TODO start a spinner here, for upload may take some time

		var file = files[0]
		var data = new FormData()
		data.append("file", file)
		
		$.ajax({
			url: "./",
			type: "POST",
			data: data,
			cache: false,
			dataType: "json",
			processData: false,
			contentType: false
		}).done(function(r) {
			if (r.status == "OK") {
				$.growl.notice({
					"title":"Die Datei wurde erfolgreich hochgeladen.",
					"message":""
				})
				$("#file-upload-form")[0].reset()
			} else {
				cbFail(r.message)
			}
		}).fail(function(r) {
			cbFail(r.message)
		})
	})
})