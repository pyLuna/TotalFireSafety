
(function ($) {
    "use strict";

    
    /*==================================================================
    [ Validate ]*/
    var input = $('.validate-input .input100');
    var message = '';
    $('.validate-form').on('submit',function(){
        var check = true;

        for(var i=0; i<input.length; i++) {
            if (validate(input[i]) == false) {
                showValidate(input[i]);
                check=false;
            }
        }
        return check;
    });


    $('.validate-form .input100').each(function(){
        $(this).focus(function(){
           hideValidate(this);
        });
    });

    function validate (input) {
        if ($(input).val().length == 0) {
            message = 'This field is required';
            return false;
        }
        else {
            if ($(input).attr('type') == 'text' && $(input).val().length < 3) {
                message = 'Minimum length of 3';
                return false;
            }
            //else if ($(input).attr('type') == 'password' && $(input).val().length < 8){
            //    message = 'Minimum length of 8';
            //    return false;
            //}
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