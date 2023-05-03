$(document).ready(function () {
	$('#NewProForm').validate({
		rules: {
			projName: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			projSubject: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			projClient: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			projLocation: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			projLead: {
				required: true
			},
			projStatus: {
				required: true
			},
			projStartDate: {
				required: true
			},
			projEndDate: {
				required: true
			}
		},
		messages: {
			projName: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			projSubject: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			projClient: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			projLocation: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			projLead: {
				required: "Project lead is required"
			},
			projStatus: {
				required: "Project lead is required"
			},
			projStartDate: {
				required: "Project lead is required"
			},
			projEndDate: {
				required: "Project lead is required"
			}
		}
	});
});