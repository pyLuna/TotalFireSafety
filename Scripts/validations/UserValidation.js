//$(document).ready(function () {
//	let username = document.getElementById("Credential.username");
//	$('#formId').validate({
//		rules: {
//			emp_name: {
//				required: true,
//				minlength: 4
//				//lettersonly: true
//			},
//			emp_contact: {
//				required: true,
//				minlength: 11
//			},
//			emp_no: {
//				required: true,
//				minlength: 8
//			},
//			emp_hiredDate: {
//				required: true,
//			},
//			emp_position: {
//				required: true,
//				minlength: 5
//				//Credential.username: {
//				//	required: true,
//				//	minlength: 3
//				//}
//			},

(function ($) {
	"use strict";

	/*==================================================================
	[ Validate ]*/
	var input = $('.input-field input');
	var dropdown = $('.input-field select');
	console.log(input);
	var message = '';
	$('#saveBtn').on('click', function () {
		var check = true;

		for (var i = 0; i < input.length; i++) {
			if (validate(input[i]) == false) {
				showValidate(input[i]);
				check = false;
			}
		}
		for (var i = 0; i < dropdown.length; i++) {
			if (validate(dropdown[i]) == false) {
				showValidate(dropdown[i]);
				check = false;
			}
		}
		return check;
	});

	$(input).each(function () {
		$(this).focus(function () {
			hideValidate(this);
		});
	});

	function validate(input) {
		if ($(input).val() == null || $(input).val().length == 0) {
			message = 'This field is required';
			return false;
		}
		else if ($(input).attr('name') == 'emp_name' && $(input).val().length < 4) {
			message = 'Minimum length of 4';
			return false;
        }
		else if ($(input).attr('name') == 'emp_contact' && $(input).val().length < 11) {
			message = 'Minimum length of 11';
			return false;
		}
		else if ($(input).attr('name') == 'emp_no' && $(input).val().length < 8) {
			message = 'Minimum length of 8';
			return false;
		}
		else if ($(input).attr('name') == 'Credential.username' && $(input).val().length < 3) {
			message = 'Minimum length of 3';
			return false;
		}
		else if ($(input).attr('name') == 'Credential.password' && $(input).val().length < 8) {
			message = 'Minimum length of 8';
			return false;
		}
	}

	function showValidate(input) {
		var thisAlert = $(input).parent();

		$(thisAlert).addClass('alert-validate');
		if (message.length != 0) {
			$(thisAlert).attr('data-validate', message);
			message = '';
		}
	}

	function hideValidate(input) {
		var thisAlert = $(input).parent();

		$(thisAlert).removeClass('alert-validate');
	}
})(jQuery);