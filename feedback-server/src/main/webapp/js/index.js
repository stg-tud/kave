require([], function() {
	
	function cbFail(msg) {
		$.growl.error({
			"title":msg,
			"message":"",
			"location":"tl"
		})
	}
	
	function cbOk(msg) {
		$.growl.notice({
			"title":msg,
			"message":"",
			"location":"tl"
		})
	}

	function enableSpinner() {
		$("#file-upload-form").hide()
		$(".spinner").show()
	}

	function disableSpinner() {
		$(".spinner").hide()
		$("#file-upload-form").show()
	}

	$("#submit-upload").removeAttr("disabled")
	
	$("#submit-upload").click(function(e) {
		e.preventDefault()
		e.stopPropagation()
		
		var confirmation = $("#confirm")[0]
		if (!confirmation.checked) {
			cbFail("Please confirm the disclaimer before submitting a file.")
			return
		}
		
		var fileInput = $("#file")[0]
		var files = fileInput.files
		if (files.length == 0) {
			cbFail("No file was selected.");
			return
		} else if (files.length > 1) {
			cbFail("Please select only one file at a time.")
			return
		}
		
		enableSpinner()

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
			disableSpinner()
			if (r.status == "OK") {
				cbOk("File upload was successful.")
				$("#file-upload-form")[0].reset()
			} else {
				cbFail(r.message)
			}
		}).fail(function(r) {
			disableSpinner()
			cbFail(r.message)
		})
	})
})