
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
            inpsizesel: {
                required: true
            },
            inpsize: {
                required: true
            },
            inpsizequant: {
                required: true
            },
            sizequant: {
                required: true
            }
        },
        messages: {
            in_name: {
                required: "Item name is required.",
                minlength: "Item name must be at least 4 characters long.",
                maxlength: "Maximun of item name are 30 characters only."
            },
            in_category: {
                required: "Category is required"
            },
            class: {
                required: "Class is required"
            },
            in_type: {
                required: "Type is required"
            },
            inpsizesel: {
                required: "Size is required"
            },
            sizesel: {
                required: ""
            },
            inpsizequant: {
                required: "Quantity is required"
            },
            sizequant: {
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
                required: "Item ID is required"
            },
            inpQuant1: {
                required: "Quantity is required"
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
                required: "Item name is required",
                minlength: "Item name must be at least 4 characters long.",
                maxlength: "Maximun of item name are 30 characters only."
            },
            selcat2: {
                required: "Category is required"
            },
            selclass2: {
                required: "Class is required"
            },
            seltype2: {
                required: "Type is required"
            },
            inpSize2: {
                required: "Size is required"
            },
            inpSize2A: {
                required: ""
            },
            inpQuant2: {
                required: "Quantity is required"
            },
            inpQuant2A: {
                required: ""
            }
        }
    });
});