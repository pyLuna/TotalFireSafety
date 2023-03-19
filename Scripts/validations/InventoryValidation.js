$(document).ready(function () {
    $('#addForm').validate({
        rules: {
            in_name: {
                required: true,
                minlength: 3
            },
            in_categort: {
                required: true
            },
            class: {
                required: true
            },
            in_type: {
                required: true
            },
            inpSizeSel: {
                required: true
            },
            inpSize: {
                required: true
            },
            inpSizeQuant: {
                required: true
            },
            sizeQuant: {
                required: true
            }
        },
        messages: {
            in_name: {
                required: "Item name must be required.",
                minlength: "Item name must be at least 3 characters long."
            },
            in_category: {
                required: "Category must be required"
            },
            class: {
                required: "Category must be required"
            },
            in_type: {
                required: "Type must be required"
            },
            inpSizeSel: {
                required: "Size must be required"
            },
            inpSize: {
                required: "Size unit must be required"
            },
            inpSizeQuant: {
                required: "Quantity must be required"
            },
            sizeQuant: {
                required: "Quantity must be required"
            }
        }
    });
});
$(document).ready(function () {
    $('#qtyForm').validate({
        rules: {
            in_code: {
                required: true
            },
            inpQuant1: {
                required: true
            }
        },
        messages: {
            in_code: {
                required: "Item ID must be required"
            },
            inpQuant1: {
                required: "Quantity must be required"
            }
        }
    });
});

//(function ($) {
//	"use strict";

//	==================================================================
//	[ Validate ]
//	var input = $('.input-field input');
//	var dropdown = $('.input-field select');
//	console.log(tableInp);
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

//	$(input).each(function () {
//		$(this).focus(function () {
//			hideValidate(this);
//		});
//	});
//	$(dropdown).each(function () {
//		$(this).focus(function () {
//			hideValidate(this);
//		});
//	});
//	function validate(input) {
//		if ($(input).val() == null || $(input).val().length == 0) {
//			message = 'This field is required';
//			return false;
//		}
//	}

//	function showValidate(input) {
//		var thisAlert = $(input).parent();

//		$(thisAlert).addClass('alert-validate');
//		if ($(input).attr('name') == 'request_emp') {
//			$(thisAlert).addClass('employee_input');
//		}
//		if ($(input).attr('name') == 'select_type') {
//			$(thisAlert).addClass('employee_dropdown');
//		}
//		if (message.length != 0) {
//			$(thisAlert).attr('data-validate', message);
//			message = '';
//		}
//		 Add style to the element with data-validation="required"
//		$('[data-validation="This field is required"]').css({
//			color: "red",
//			fontWeight: "bold"
//		});
//	}

//	function hideValidate(input) {
//		var thisAlert = $(input).parent();

//		$(thisAlert).removeClass('alert-validate');
//	}
//})(jQuery);