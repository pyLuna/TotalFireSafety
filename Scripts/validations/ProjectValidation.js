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
				required: "Baseline Finish is required"
			}
		}
	});

	/*START DATE AND BASELINE FINISH VALIDATION*/
	$('input[name="projEndDate"]').change(function () {
		var startDate = $('input[name="projStartDate"]').val();
		var endDate = $(this).val();

		if (startDate === endDate) {
			/*alert("Start date and end date cannot be the same.");*/
			$(".emErrP").html("Start date and Baseline Finish cannot be the same.");
			/*$(this).val('');*/
		}
		/*else if (startDate > endDate){
			$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
		}*/
		else {
			$(".emErrP").html("");
		}
		return startDate
		/*if (startDate > endDate) {
			alert("Invalid input! Baseline Finish is ahead to start date. ");
			$(this).val('');
			$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
		}
		else {
			$(".emErrP").html("");
		}

		return startDate;*/
	});

	$('input[name="projStartDate"]').change(function () {
		var endDate = $('input[name="projEndDate"]').val();
		var startDate = $(this).val();

		if (endDate === startDate) {
			/*alert("Start date and end date cannot be the same.");*/
			$(".emErrP").html("Start date and Baseline Finish cannot be the same.");
			/*$(this).val('');*/
		}
		else {
			$(".emErrP").html("");
		}
		return endDate
		/*
				if (startDate > endDate) {
					alert("Invalid input! Baseline Finish is ahead to start date. ");
					$(this).val('');
					$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
				}
				else {
					$(".emErrP").html("");
				}
				return endDate;*/
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
				maxlength: 250
			},
			proj_subject: {
				required: true,
				minlength: 5,
				maxlength: 250
			},
			proj_client: {
				required: true,
				minlength: 5,
				maxlength: 250
			},
			proj_location: {
				required: true,
				minlength: 5,
				maxlength: 250
			},
			proj_lead: {
				required: true
			},
			proj_status: {
				required: true
			},
			proj_strDate: {
				required: true
			},
			proj_endDate: {
				required: true
			}
		},
		messages: {
			proj_name: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 250 characters only."
			},
			proj_subject: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 250 characters only."
			},
			proj_client: {
				required: "Project name is required.",
				minlength: "Project name must be at least 5 characters long.",
				maxlength: "Maximum length is 250 characters only."
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
			},
			proj_strDate: {
				required: "Project lead is required"
			},
			proj_endDate: {
				required: "Baseline Finish is required"
			}
		}
	});

	$('input[name="proj_endDate"]').change(function () {
		var startDate = $('input[name="proj_strDate"]').val();
		var endDate = $(this).val();

		if (startDate === endDate) {
			$(this).val('');
			$(this).addClass('error');
			$('<span class="error-msg">Start date and end date cannot be the same.</span>').insertAfter($(this));
		} else {
			$(this).removeClass('error');
			$(this).siblings('.error-msg').remove();
		}
	});
});

/*======================= END ==============================*/
/*UPDATE report VALIDATION*/

$(document).ready(function () {
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
});

/*======================= END ==============================*/