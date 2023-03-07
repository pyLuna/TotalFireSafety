$(document).ready(function () {
    $('#addForm').validate({
        rules: {
            itemName: {
                required: true,
                minlength: 4
            }
        },
        messages: {
            itemName: {
                required: "Please enter item name.",
                minlength: "Item name must be at least 4 characters long."
            }
        }
    });
});