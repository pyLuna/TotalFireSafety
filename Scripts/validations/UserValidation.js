

$(document).on('keypress', '#emp_contact', function (event) {
	var length = this.value.length;
	if (length >= 11) {
		event.preventDefault();
		return false;
	}
});

$(document).on('keypress', '#emp_name', function (event) {
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
			emp_name: {
				required: true,
				minlength: 4,
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
			sel_roles: {
				required: true
			},
			sel_stats: {
				required: true
			}
		},
		messages: {
			emp_name: {
				required: "Please enter name.",
				minlength: "Name must be at least 4 characters long.",
				maxlength: "Maximum length of name is 30 characters only."
			},
			emp_contact: {
				required: "Please enter contact number.",
				minlength: "Contact number must be at least 11 numbers long.",
				maxlength: "Contact number is 11 numbers long only."
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
			},
			sel_roles: {	
				required: "Please enter roles"
			},
			sel_stats: {
				required: "Please enter status"
			}
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

	$("#addBtn").on('click', function (e) {
		$("#add-popup").addClass("add-popup-show");
		$("#add-popup")
			.find(".form-contents")
			.html(add_user_pop_up_form)
			.closest("form")
			.validate();
	})
});
