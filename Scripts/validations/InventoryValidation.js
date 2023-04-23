
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
            inpsizesel: {
                required: "Size must be required"
            },
            sizesel: {
                required: ""
            },
            inpsizequant: {
                required: "Quantity must be required"
            },
            sizequant: {
                required: ""
            }
        }
    });
});

//$(document).ready(function () {
//    const pop_up_form = `
//                                    <div class="fields">
//                                            <div class="input-field">
//                                                <label>item id</label>
//                                                <input type="text" id="in_code" name="in_code" placeholder="abc-def-ghi-000" readonly>
//                                            </div>
//                                        <div class="input-field">
//                                            <label>item name</label>
//                                            <input type="text" id="in_name" name="in_name" placeholder="enter item name" required>
//                                        </div>
//                                        <div class="input-field">
//                                            <label>category</label>
//                                            <input type="text" id="in_category" name="in_category" list="catlist" placeholder="enter item category" required>
//                                            <datalist id="catlist"></datalist>
//                                        </div>
//                                        <div class="input-field">
//                                            <label>class</label>
//                                            <select id="class" name="class" required>
//                                                <option value="" selected>select class</option>
//                                                <option>v1</option>
//                                            </select>
//                                        </div>
//                                        <input type="text" id="in_class" name="in_class" style="display:none;">
//                                        <div class="input-field">
//                                            <label>type</label>
//                                            <input type="text" id="in_type" list="typelist" name="in_type" placeholder="enter item type" required>
//                                            <datalist id="typelist"></datalist>
//                                        </div>
//                                        <div class="input-field">
//                                            <label>size</label>
//                                            <div class="input-field-size">
//                                                <input type="number" id="inpsizesel" name="inpsizesel" placeholder="00000" required>
//                                                <select id="sizesel" name="sizesel" required>
//                                                    <option value="" selected>unit</option>
//                                                    <option>Inch</option>
//                                                    <option>cm</option>
//                                                </select>
//                                            </div>
//                                            <label id="inpsizesel-error" class="error manual-error" for="inpsizesel"></label>
//                                            <label id="sizesel-error" class="error manual-error" for="sizesel"></label>
//                                        </div>
//                                        <input type="text" id="in_size" name="in_size" style="display:none;" required>
//                                        <div class="input-field">
//                                            <label>quantity</label>
//                                            <div class="input-field-quantity">
//                                                <input type="number" id="inpsizequant" name="inpsizequant" placeholder="00000" required>
//                                                <select id="sizequant" name="sizequant" required>
//                                                    <option value="" selected>unit</option>
//                                                    <option>pcs</option>
//                                                </select>
//                                            </div>
//                                            <label id="inpsizequant-error" class="error manual-error" for="inpsizequant"></label>
//                                            <label id="sizequant-error" class="error manual-error" for="sizequant"></label>
//                                        </div>
//                                        <input type="text" id="in_quantity" name="in_quantity" style="display:none!important;" required>
//                                        <input type="text" id="in_remarks" name="in_remarks" style="display:none;" required>
//                                        <div class="input-field">
//                                            <label>date added</label>
//                                            <input type="text" id="dateadded" name="in_dateadded" readonly>
//                                        </div>
//                                        <div class="input-field">
//                                            <div class="img-bar-grp" id="image-container">
                                            
//                                            </div>
//                                        </div>
//                                    </div>`

//    $("#add_new_itm").on('click', function (e) {
//        $("#add-popup-inv").addClass("add-popup-inv-show");
//        $("#add-popup-inv")
//            .find(".inventory-form-contents")
//            .html(pop_up_form)
//            .closest("form")
//            .validate();
//    })
//});
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

//$(document).ready(function () {
//    const add_quan_pop_up_form = ``

//    $("#add_quan11").on('click', function (e) {
//        $("#add-qty-popup-inv").addClass("add-qty-popup-inv-show");
//        $("#add-qty-popup-inv")
//            .find(".inventory-form-contents")
//            .html(add_quan_pop_up_form)
//            .closest("form")
//            .validate();
//    })
//});
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