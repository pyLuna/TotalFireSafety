$(document).ready(function () {
	$('#formId').validate({
		rules: {
			emp_name: {
				required: true,
				minlength: 10
			}
		},
		messages: {
			emp_name: {
				required: "Please enter your name.",
				minlength: "Your name must be at least 10 characters long."
			}
		}
	});
});