/*ADD NEW PROJECT VALIDATION */

$(document).ready(function () {
	$('#ManPropoForm').validate({
		rules: {
			Engineer: {
				required: true
			},
			proj_subject: {
				required: true
			},
			,
			'propInput[]': { // Assuming 'propInput' is the name attribute of the table cells
				required: function (element) {
					return $('#propInput tbody tr').length === 0; // Require if there are no rows in the table
				}
		},
		messages: {
			Engineer: {
				required: "Please select an engineer.",
			}
		}
	});
});

/*START DATE AND BASELINE FINISH VALIDATION*/
/*$('input[name="projEndDate"]').change(function () {
	var startDate = $('input[name="projStartDate"]').val();
	var endDate = $(this).val();

	if (startDate === endDate) {
		alert("Start date and end date cannot be the same.");
		$(".emErrP").html("Start date and Baseline Finish cannot be the same.");
		$(this).val('');
	}
	else if (startDate > endDate) {
		$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
	}
	else {
		$(".emErrP").html("");
	}
	return startDate
	if (startDate > endDate) {
		alert("Invalid input! Baseline Finish is ahead to start date. ");
		$(this).val('');
		$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
	}
	else {
		$(".emErrP").html("");
	}

	return startDate;
});


$('input[name="projStartDate"]').change(function () {
	var endDate = $('input[name="projEndDate"]').val();
	var startDate = $(this).val();

	if (endDate === startDate) {
		alert("Start date and end date cannot be the same.");
		$(".emErrP").html("Start date and Baseline Finish cannot be the same.");
		$(this).val('');
	}
	else {
		$(".emErrP").html("");
	}
	return endDate

	if (startDate > endDate) {
		alert("Invalid input! Baseline Finish is ahead to start date. ");
		$(this).val('');
		$(".emErrP").html("Invalid input! Baseline Finish is ahead to start date.");
	}
	else {
		$(".emErrP").html("");
	}
	return endDate;
});
*/
/*======================= END ==============================*/