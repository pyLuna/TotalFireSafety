

$(document).on('keypress', '#emp_name', function (event) {
	var regex = new RegExp("^[a-zA-Z ]+$");
	var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
	if (!regex.test(key)) {
		event.preventDefault();
		return false;
	}
});

$(document).ready(function () {
	let username = document.getElementById("Credential.username");
	$('#formId').validate({
		rules: {
			emp_name: {
				required: true,
				minlength: 4,
				maxlength: 20
			},
			emp_contact: {
				required: true,
				minlength: 11,
				maxlength: 11
			},
			emp_no: {
				required: true,
				minlength: 8
			},
			emp_hiredDate: {
				required: true,
			},
			emp_position: {
				required: true,
				minlength: 5
			}
		},
		messages: {
			emp_name: {
				required: "Please enter name.",
				minlength: "Name must be at least 4 characters long.",
				maxlength: "Maximum length of name is 20 characters only."
			},
			emp_contact: {
				required: "Please enter contact number.",
				minlength: "Contact number must be at least 11 numbers long.",
				maxlength: "Contact number is 11 numbers long only."
			},
			emp_no: {
				required: "Please enter employee ID",
				minlength: "Employee ID must be at least 8 numbers."
			},
			emp_hiredDate: {
				required: "This field is required.",
			},
			emp_position: {
				required: "Please enter position",
				minlength: "Position must be at least 5 numbers."
			}
		}
	});
	$('#Credential\\.username').rules('add', {
		required: true,
		minlength: 3,
		messages: {
			required: "Please enter username.",
			minlength: "Username must be at least 3 characters."
		}
	});
	$('#Credential\\.username').on('keyup', function () {
		$(this).valid();
	});
	$('#Credential\\.password').rules('add', {
		required: true,
		minlength: 3,
		messages: {
			required: "Please enter password",
			minlength: "Password must be at least 3 characters."
		}
	});
	$('#Credential\\.password').on('keyup', function () {
		$(this).valid();
	});
});
//(function ($) {
//	"use strict";

//	/*==================================================================
//	[ Validate ]*/
//	var input = $('.input-field input');
//	var dropdown = $('.input-field select');
//	console.log(input);
//	var message = '';
//	$('#saveBtn').on('click', function () {
//		var check = true;

//		for (var i = 0; i < input.length; i++) {
//			if (validate(input[i]) == false) {
//				showValidate(input[i]);
//				check = false;
//			}
//		}
//		for (var i = 0; i < dropdown.length; i++) {
//			if (validate(dropdown[i]) == false) {
//				showValidate(dropdown[i]);
//				check = false;
//			}
//		}
//		return check;
//	});
//	function validate(input) {
//		if ($(input).val() == null || $(input).val().length == 0) {
//			message = 'This field is required';
//			return false;
//		}
//		else if ($(input).attr('name') == 'emp_name' && $(input).val().length < 4) {
//			message = 'Minimum length of 4';
//			return false;
//        }
//		else if ($(input).attr('name') == 'emp_contact' && $(input).val().length < 11) {
//			message = 'Minimum length of 11';
//			return false;
//		}
//		else if ($(input).attr('name') == 'emp_no' && $(input).val().length < 8) {
//			message = 'Minimum length of 8';
//			return false;
//		}
//		else if ($(input).attr('name') == 'Credential.username' && $(input).val().length < 3) {
//			message = 'Minimum length of 3';
//			return false;
//		}
//		else if ($(input).attr('name') == 'Credential.password' && $(input).val().length < 8) {
//			message = 'Minimum length of 8';
//			return false;
//		}
//	}

//	function showValidate(input) {
//		var thisAlert = $(input).parent();

//		$(thisAlert).addClass('alert-validate');
//		if (message.length != 0) {
//			$(thisAlert).attr('data-validate', message);
//			message = '';
//		}
//	}

//	function hideValidate(input) {
//		var thisAlert = $(input).parent();

//		$(thisAlert).removeClass('alert-validate');
//	}
//})(jQuery);