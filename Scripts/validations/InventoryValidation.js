
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
    $("#inpSizeQuant").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
    $("#Inv_Limitsmaximum").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
    $("#Inv_Limitsminimum").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
    $("#Inv_Limitsreorder").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
    $("#batchQuant").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
    $("#quantVal").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });
});

/*$(document).ready(function () {
    $('input[name="inpSizeQuant"]').change(function () {
        *//*var endDate = $('input[name="projEndDate"]').val();*//*
var invtQuan = $(this).val();

if (invtQuan > 200) {
    *//*alert("150 is the maximum");*//*
$(".emErr").html("Quantity must under 200.");
*//*$(this).val('');*//*
}
else {
$(".emErr").html("");
}

});
});*/


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
            inpSizeQuant: {
                required: true,
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
            inpSizeQuant: {
                required: "Quantity is required",
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
    $("#inpQuant1").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });

});

/*$(document).ready(function () {
    $('input[name="inpQuant1"]').change(function () {
        *//*var endDate = $('input[name="projEndDate"]').val();*//*
var invtQuan = $(this).val();

if (invtQuan > 200) {
    *//*alert("150 is the maximum");*//*
$(".emErr").html("Quantity must under 200.");
*//*$(this).val('');*//*
}
else {
$(".emErr").html("");
}

});
});
*/
$(document).ready(function () {
    $('#qtyForm').validate({
        rules: {
            in_code: {
                required: true
            },
            inpQuant1: {
                required: true,
                max: 200
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
                required: "Quantity is required",
                max: "Quantity must not exceed 200"
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
    $("#inpQuant2").keypress(function (e) {
        var length = this.value.length;
        if (length >= 3) {
            e.preventDefault();
            /*alert("not allow more than 3 character");*/
        }
    });

});


/*$(document).ready(function () {
    $('input[name="inpQuant2"]').change(function () {
        *//*var endDate = $('input[name="projEndDate"]').val();*//*
var invtQuan = $(this).val();

if (invtQuan > 200) {
    *//*alert("150 is the maximum");*//*
$(".emErr").html("Quantity must under 200.");
*//*$(this).val('');*//*
}
else {
$(".emErr").html("");
}

});
});
*/
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
                required: true,
                max: 200
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
                required: "Quantity is required",
                max: "Quantity must not exceed 200"
            },
            inpQuant2A: {
                required: ""
            }
        }
    });
});