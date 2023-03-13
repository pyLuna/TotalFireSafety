//$(document).ready(function () {
//    $('#addForm').validate({
//        rules: {
//            itemName: {
//                required: true,
//                minlength: 4
//            }
//        },
//        messages: {
//            itemName: {
//                required: "Please enter item name.",
//                minlength: "Item name must be at least 4 characters long."
//            }
//        }
//    });
//});

(function ($) {
	"use strict";

	/*==================================================================
	[ Validate ]*/
	var input = $('.input-field input');
	var dropdown = $('.input-field select');
	console.log(tableInp);
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
	$(dropdown).each(function () {
		$(this).focus(function () {
			hideValidate(this);
		});
	});
	function validate(input) {
		if ($(input).val() == null || $(input).val().length == 0) {
			message = 'This field is required';
			return false;
		}
	}

	function showValidate(input) {
		var thisAlert = $(input).parent();

		$(thisAlert).addClass('alert-validate');
		if ($(input).attr('name') == 'request_emp') {
			$(thisAlert).addClass('employee_input');
		}
		if ($(input).attr('name') == 'select_type') {
			$(thisAlert).addClass('employee_dropdown');
		}
		if (message.length != 0) {
			$(thisAlert).attr('data-validate', message);
			message = '';
		}
		// Add style to the element with data-validation="required"
		//$('[data-validation="This field is required"]').css({
		//	color: "red",
		//	fontWeight: "bold"
		//});
	}

	function hideValidate(input) {
		var thisAlert = $(input).parent();

		$(thisAlert).removeClass('alert-validate');
	}
})(jQuery);