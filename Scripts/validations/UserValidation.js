$(document).ready(function () {
	$("#emp_contact").keypress(function (e) {
		var length = this.value.length;
		if (length >= 11) {
			e.preventDefault();
			/*alert("not allow more than 11 character");*/
		}
	});

});

$(document).on('keypress', '#emp_Sname', function (event) {
	var regex = new RegExp("^[a-zA-Z ]+$");
	var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
	if (!regex.test(key)) {
		event.preventDefault();
		return false;
	}
});

$(document).on('keypress', '#emp_Fname', function (event) {
	var regex = new RegExp("^[a-zA-Z ]+$");
	var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
	if (!regex.test(key)) {
		event.preventDefault();
		return false;
	}
});

$(document).on('keypress', '#emp_Mname', function (event) {
	var regex = new RegExp("^[a-zA-Z ]+$");
	var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
	if (!regex.test(key)) {
		event.preventDefault();
		return false;
	}
});

$(document).ready(function () {
	let username = document.getElementById("Credential.username");
	/*$("#sel-roles").attr('name', 'sel_roles');*/
	$('#formId').validate({
		rules: {
			emp_lname: {
				required: true,
				minlength: 3,
				maxlength: 30
			},
			emp_fname: {
				required: true,
				minlength: 3,
				maxlength: 30
			},
			emp_name: {
				required: true,
				minlength: 3,
				maxlength: 30
			},
			emp_contact: {
				required: true,
				minlength: 11,
				maxlength: 11
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
			},
			//sel_roles: {
			//	required: true
			//},
			//sel_stats: {
			//	required: true
			//}
		},
		messages: {
			emp_lname: {
				required: "Surname is required.",
				minlength: "Surname must be at least 3 characters long.",
				maxlength: "Maximum length of name is 30 characters only."
			},
			emp_fname: {
				required: "First name is required.",
				minlength: "First name must be at least 3 characters long.",
				maxlength: "Maximum length of name is 30 characters only."
			},
			emp_name: {
				required: "Middle name is required.",
				minlength: "Middle name must be at least 3 characters long.",
				maxlength: "Maximum length of name is 30 characters only."
			},
			emp_contact: {
				required: "contact number is required.",
				minlength: "Contact number must be at least 11 numbers long.",
				maxlength: "Contact number is 11 numbers long only."
			},
			emp_no: {
				required: "Employee ID is required",
				minlength: "Employee ID must be at least 8 numbers."
			},
			emp_hiredDate: {
				required: "Hired date is is required.",
			},
			emp_position: {
				required: "Position is required.",
				minlength: "Position must be at least 5 numbers."
			},
			//sel_roles: {
			//	required: "Please enter roles"
			//},
			//sel_stats: {
			//	required: "Please enter status"
			//}
		}
	});

	$('#Credential\\.username').rules('add', {
		required: true,
		minlength: 5,
		maxlength: 12,
		messages: {
			required: "Username is required.",
			minlength: "Username must be at least 5 characters.",
			maxlength: "Maximum length of username is 12 characters only."
		}
	});
	$('#Credential\\.username').on('keyup', function () {
		$(this).valid();
	});
	$('#Credential\\.password').rules('add', {
		required: true,
		minlength: 8,
		maxlength: 16,
		messages: {
			required: "Password is required.",
			minlength: "Password must be at least 8 characters.",
			maxlength: "Maximum length of password is 16 characters only."
		}
	});
	$('#Credential\\.password').on('keyup', function () {
		$(this).valid();
	});
});