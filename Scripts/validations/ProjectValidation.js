/*ADD NEW PROJECT VALIDATION */

$(document).ready(function () {
	$('#NewProForm').validate({
		rules: {
			projName: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			projSubject: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			projClient: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			projLocation: {
				required: true,
				minlength: 5,
				maxlength: 50
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
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			projSubject: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			projClient: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			projLocation: {
				required: "Project name is required.",
				minlength: "Project name must  at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
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
				minlength: 5,
				maxlength: 50
			},
			proj_subject: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			proj_client: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			proj_location: {
				required: true,
				minlength: 5,
				maxlength: 50
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
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			proj_subject: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			proj_client: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			proj_location: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
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
/*UPDATE PROJECT VALIDATION*/

/*$(document).ready(function () {
	$('#SubmitProForm').validate({
		rules: {
			rep_date: {
				required: true
			},
			rep_status: {
				required: true
			},
			rep_scope: {
				required: true,
				minlength: 5,
				maxlength: 50
			},
			rep_description: {
				required: true,
				minlength: 5
			}
		},
		messages: {
			rep_date: {
				required: "Report date is required"
			},
			rep_status: {
				required: "Report Status is required"
			},
			rep_scope: {
				required: "Report Scope is required",
				minlength: "Report scope must be at least 5 characters long.",
				maxlength: "Maximum length is 50 characters only."
			},
			rep_description: {
				required: "Report scope is required",
				minlength: "Report scope must be at least 5 characters long."
			}
		}
	});
});*/

/*======================= END ==============================*/

/*========TRIGGER BUTTONS===================*/

/*CREATE PROJECT*/
$(document).ready(function () {
	$("#save-button").click(function () {
		if ($('#NewProForm').valid()) {
			savePro();
			location.reload();
		}
	});
})

/*ADD REPORT BUTTON*/
$(document).ready(function () {
	$("#addRepBtn").click(function () {
		SaveData1();
		saveReport();
		saveData();
	});
})

/*UPDATE PROJECT*/
$(document).ready(function () {
	$("#UpdtProj").click(function () {
		if ($('#UpdateProForm').valid()) {
			updatedata()
		}
	});
})

