
/*ADD NEW ITEM VALIDATION SECTION*/

//$(document).on('keypress', '#in_name', function (event) {
//    var regex = new RegExp("^[a-zA-Z ]+$");
//    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
//    if (!regex.test(key)) {
//        event.preventDefault();
//        return false;
//    }
//})

$(document).ready(function () {
    $('#addForm').validate({
        rules: {
            in_name: {
                required: true,
                minlength: 4,
                maxlength: 30
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
                minlength: "Item name must be at least 4 characters long.",
                maxlength: "Maximun of item name are 30 characters only."
            },
            in_category: {
                required: "Category must be required"
            },
            class: {
                required: "Class must be required"
            },
            in_type: {
                required: "Type must be required"
            },
            inpSizeSel: {
                required: "Size must be required"
            },
            sizeSel: {
                required: ""
            },
            inpSizeQuant: {
                required: "Quantity must be required"
            },
            sizeQuant: {
                required: ""
            }
        }
    });
});

 /*======================= END ==============================*/

/*ADD QUANTITY VALIDATION SECTION*/

$(document).ready(function () {
    $('#qtyForm').validate({
        rules: {
            in_code: {
                required: true
            },
            inpQuant1: {
                required: true
            },
            inpQuant: {
                required: true
            }
        },
        messages: {
            in_code: {
                required: "Item ID must be required"
            },
            inpQuant1: {
                required: "Quantity must be required"
            },
            inpQuant: {
                required: ""
            }
        }
    });
});
/*======================= END ==============================*/

/*EDIT ITEM VALIDATION SECTION*/

$(document).ready(function () {
    $('#editForm').validate({
        rules: {
            in_name: {
                required: true,
                minlength: 4,
                maxlength: 30
            },
            selcat2: {
                required: true
            },
            selclass2: {
                required: true
            },
            seltype2: {
                required: true
            },
            inpSize2: {
                required: true
            },
            inpSize2A: {
                required: true
            },
            inpQuant2: {
                required: true
            },
            inpQuant2A: {
                required: true
            }
        },
        messages: {
            in_name: {
                required: "Item name must be required",
                minlength: "Item name must be at least 4 characters long.",
                maxlength: "Maximun of item name are 30 characters only."
            },
            selcat2: {
                required: "Category must be required"
            },
            selclass2: {
                required: "Class must be required"
            },
            seltype2: {
                required: "Type must be required"
            },
            inpSize2: {
                required: "Size must be required"
            },
            inpSize2A: {
                required: ""
            },
            inpQuant2: {
                required: "Quantity must be required"
            },
            inpQuant2A: {
                required: ""
            }
        }
    });
});
/*======================= END ==============================*/

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