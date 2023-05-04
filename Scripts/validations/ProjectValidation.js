
/*ADD NEW PROJECT VALIDATION */

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
				required: "Project Status is required"
			},
			projStartDate: {
				required: "Project Start Date is required"
			},
			projEndDate: {
				required: "Project End Date is required"
			}
		}
	});
});

/*======================= END ==============================*/

/*UPDATE PROJECT VALIDATION*/

$(document).ready(function () {
	$('#UpdateProForm').validate({
		rules: {
			proj_name: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			proj_subject: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			proj_client: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			proj_location: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			proj_lead: {
				required: true
			},
			proj_status: {
				required: true
			}/*,
			proj_strDate: {
				required: true
			}*/,
			proj_endDate: {
				required: true
			}
		},
		messages: {
			proj_name: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			proj_subject: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			proj_client: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			proj_location: {
				required: "Project name is required.",
				minlength: "Project name be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			proj_lead: {
				required: "Project lead is required"
			},
			proj_status: {
				required: "Project Status is required"
			}/*,
			proj_strDate: {
				required: "Project lead is required"
			}*/,
			proj_endDate: {
				required: "Baseline Finish is required"
			}
		}
	});
});

/*======================= END ==============================*/