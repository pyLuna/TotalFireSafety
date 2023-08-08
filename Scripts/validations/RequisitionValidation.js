//$(document).ready(function () {
//    $('#create-form').validate({
//        rul
//        },
//        messages: {
           
//        }
//    });
//})

//$(document).ready(function () {
//    $('#input_quan_form').validate({
//        rules: {
//            quantVal: {
//                required: true

//            }
//        },
//        messages: {
//            quantVal: {
//                required: "Quantity must be required!"
//            }
//        }
//    });
//});

//$("button[type='submit']").on("click", function () {
//    $("#input_quan_form").valid();
//});






//$(document).ready(function () {
//    $('#formId').validate({
//        rules: {
//            request_emp: {
//                required: true,
//                minlength: 4
//            },
//            select_type: {
//                required: true,
//            }
//        },
//        messages: {
//            request_emp: {
//                required: "Please enter name.",
//                minlength: "name must be at least 4 characters long."
//            },
//            select_type: {
//                required: "Request type must be required."
//            }
//        }
//    });
//});

//(function ($) {
//	"use strict";

//	==================================================================
//	[ validate ]
//	var input = $('.input-field input');
//	var dropdown = $('.input-field select');
//	var tableinp = $('tbody td input')
//	console.log(tableinp);
//	console.log(input);
//	var message = '';

//	function addvalidationlisteners(row) {
//		var inputs = $(row).find('.input-field input');
//		var dropdowns = $(row).find('.input-field select');

//		inputs.each(function () {
//			$(this).on('input', function () {
//				validate(this);
//			});
//		});

//		dropdowns.each(function () {
//			$(this).on('change', function () {
//				validate(this);
//			});
//		});
//	}

//	$('#savebtn').on('click', function () {
//		var check = true;

//		for (var i = 0; i < tableinp.length; i++) {
//			if (validate(tableinp[i]) == false) {
//				showvalidate(tableinp[i]);
//				check = false;
//			}
//		}

//		for (var i = 0; i < input.length; i++) {
//			if (validate(input[i]) == false) {
//				showvalidate(input[i]);
//				check = false;
//			}
//		}
//		for (var i = 0; i < dropdown.length; i++) {
//			if (validate(dropdown[i]) == false) {
//				showvalidate(dropdown[i]);
//				check = false;
//			}
//		}
//		return check;
//	});

//	$(input).each(function () {
//		$(this).focus(function () {
//			hidevalidate(this);
//		});
//	});
//	$(dropdown).each(function () {
//		$(this).focus(function () {
//			hidevalidate(this);
//		});
//	});
//	function validate(input) {
//		if ($(input).val() == null || $(input).val().length == 0) {
//			message = 'this field is required';
//			return false;
//		}
//	}

//	function showvalidate(input) {
//		var thisalert = $(input).parent();

//		$(thisalert).addclass('alert-validate');
//		if ($(input).attr('name') == 'request_emp') {
//			$(thisalert).addclass('employee_input');
//		}
//		if ($(input).attr('name') == 'select_type') {
//			$(thisalert).addclass('employee_dropdown');
//		}
//		if (message.length != 0) {
//			$(thisalert).attr('data-validate', message);
//			message = '';
//		}
//		// add style to the element with data-validation="required"
//		//$('[data-validation="this field is required"]').css({
//		//	color: "red",
//		//	fontweight: "bold"
//		//});
//	}

//	function hidevalidate(input) {
//		var thisalert = $(input).parent();

//		$(thisalert).removeclass('alert-validate');
//	}
//})(jquery);

