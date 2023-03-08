$(document).ready(function () {
	let username = document.getElementById("Credential.username");
	$('#formId').validate({
		rules: {
			emp_name: {
				required: true,
				minlength: 4
				//lettersonly: true
			},
			emp_contact: {
				required: true,
				minlength: 11
			},
			emp_no: {
				required: true,
				minlength: 8
			},
			emp_hiredDate: {
				required: true,
			},
			emp_position: {
				required: true,
				minlength: 5
			}
			//Credential.username: {
			//	required: true,
			//	minlength: 3
			//}
		},
		messages: {
			emp_name: {
				required: "Please enter name.",
				minlength: "Name must be at least 4 characters long."
				//lettersonly: "Name must be letters only"
			},
			emp_contact: {
				required: "Please enter contact number.",
				minlength: "Contact number must be at least 11 characters long."
			},
			emp_no: {
				required: "Please enter employee ID",
				minlength: "Employee ID must be at least 8 numbers."
			},
			emp_hiredDate: {
				required: "This field is required.",
			},
			emp_position: {
				required: "Please enter position",
				minlength: "Position must be at least 5 numbers."
			}
			//Credential.username: {
			//required: "Please enter username.",
			//minlength: "Your name must be at least 8 numbers."
			//}
		}
	});
	$('#Credential\\.username').rules('add', {
		required: true,
		minlength: 3,
		messages: {
			required: "Please enter username.",
			minlength: "Username must be at least 3 characters."
		}
	});
	$('#Credential\\.username').on('keyup', function () {
		$(this).valid();
	});
	$('#Credential\\.password').rules('add', {
		required: true,
		minlength: 3,
		messages: {
			required: "Please enter password",
			minlength: "Password must be at least 3 characters."
		}
	});
	$('#Credential\\.password').on('keyup', function () {
		$(this).valid();
	});
	//$("#emp_name").on('input',function(){
	//	var expression = /[^a-za-z]/g;

	//});
	
});
