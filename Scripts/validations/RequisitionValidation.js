$(document).ready(function () {
    $('#formId').validate({
        rules: {
            request_emp: {
                required: true,
                minlength: 4
            },
            select_type: {
                required: true,
            }
        },
        messages: {
            request_emp: {
                required: "Please enter name.",
                minlength: "Name must be at least 4 characters long."
            },
            select_type: {
                required: "This field is required."
            }
        }
    });
});