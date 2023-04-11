let viewData = [];
let tableData = [];
let jsonArray = [];
let filtered = [];
let fixedArray = [];
let allRequests = [];
let Employees = [];
let newEmployee = [];
let itemInv = [];
let invList = [];
var crtable = document.querySelector('#create-table tbody');
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');
let addItem = document.getElementById('add-item');
let table = document.querySelector('#myTable tbody');
let emp_role = localStorage.getItem('emp_role');
let emp_name = localStorage.getItem('emp_name');
let message = "";


const template = {
	"request_id": null,
	"request_type": "",
	"request_item": "",
	"request_item_quantity": "",
	"request_date": "",
	"request_employee_id": "",
	"request_status": "",
	"request_type_id": 0
};

const template2 = {
	"request_type": "",
	"request_date": "",
	"request_employee_id": "",
	"request_type_id": 0
};

addItem.addEventListener("click", function (event) {
	event.preventDefault();
	InvsetTable(itemInv);
});
document.getElementById('create-table').addEventListener("click", function (event) {
	const target = event.target;

	if (target.tagName.toLowerCase() === "a" && target.classList.contains("delete-row")) {
		const row = target.parentNode.parentNode;
		row.parentNode.removeChild(row);
	}
});
document.getElementById('cnfBtn').addEventListener("click", function () {
	var body = setTemplate('#create-fields label', 'add');
	localStorage.setItem('bodyData', JSON.stringify(body));
	localStorage.setItem('typeData', 'add');
});

function GetAllItem() {
	fetch('/Admin/FindDataOf?requestType=inventory')
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				console.log("error fetch");
			}
		})
		.then(data => {
			invList.length = 0;
			invList.push(data);
			fixArray(invList, 3);
			console.log(`inventory`);
		})
		.catch(error => {
			window.location.replace('/Error/InternalServerError');
			console.error(error);
		});
}
function GetAllEmployee() {
	fetch('/Admin/SearchEmployee')
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				console.log("error fetch");
			}
		})
		.then(data => {
			Employees.push(data);
			fixArray(Employees, 2);
			console.log(`Employee`);
		})
		.catch(error => {
			window.location.replace('/Error/InternalServerError');
			console.error(error);
		});
}
function GetAll() {
	fetch('/Admin/FindDataOf?requestType=inventory')
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				// Handle error if unsuccessful
				let table = document.querySelector('#myTable tbody');
				// clear table
				table.innerHTML = " ";
				// table style
				let errorMessageRow = document.createElement('tr');
				errorMessageRow.style.textAlign = "center";
				errorMessageRow.style.fontStyle = "italic";
				errorMessageRow.innerHTML = '<td colspan="6">Loading Error<td>';
				table.appendChild(errorMessageRow);
			}
		})
		.then(data => {
			jsonArray.length = 0;
			jsonArray.push(data);
			fixArray(jsonArray, 1);
			DeleteCertainPart();
			if (table !== null) {
				var result = MergeSameId(fixedArray);
				setTable(result);
			}
		})
		.catch(error => {
			//window.location.replace('/Error/InternalServerError');
			console.error(error);
		});
}

function DeleteCertainPart() {
	newArray = fixedArray.map(function (item) {
		return { ...item, Inventory: undefined };
	});
}

//populate table on html
function MergeSameId(array) {
	//viewData.length = 0;
	array.length = 0;
	let uniqueIdentifier = {};
	for (var i = 0; i < allRequests.length; i++) {
		// Check if the current request ID is already in the uniqueIdentifier object
		if (!uniqueIdentifier[allRequests[i].Id]) {
			// If not, add the request to the viewData array and the uniqueIdentifier object
			array.push(allRequests[i]);
			uniqueIdentifier[allRequests[i].Id] = true;
		}
	}
	return array;
}

function InvsetTable(array) {
	let invTable = document.querySelector('#InvTable tbody');
	invTable.innerHTML = '';
	for (var i = 0; i < array.length; i++) {
		let remarks = "";
		let holder = array[i].in_remarks === null ? "" : array[i].in_remarks;
		let remclass = "";
		if (holder.trim() == 'critical') {
			remarks = 'Critical';
			remclass = 'stat-re-order';
			var row = `<tr onclick="openInputQTYForm('${array[i].in_code}')">`;
			row += `<td> ${array[i].in_name}</td>`;
			row += `<td><label>${array[i].in_category}</label></td>`;
			row += `<td><label>${array[i].in_type}</label></td>`;
			row += `<td><label>${array[i].in_size}</label></td>`;
			row += `<td><label class="${remclass}" style="font-weight:bold;">${remarks}</label></td>`;
			row += `</tr>`;
			invTable.innerHTML += row;
		}
	}
}
function extractNum(value) {
	let num = 0;
	let measurement = '';
	for (let i = 0; i < value?.length; i++) {
		if (!isNaN(parseInt(value[i]))) {
			num = num * 10 + parseInt(value[i]);
		} else if (value[i] !== ' ') {
			measurement += value[i];
		}
	}
	return { num, measurement };
}

function DataToAdd(quantity, itemCode) {
	filterArray1(itemCode.toLowerCase());

	var increment = localStorage.getItem('increment');
	var newNum = 0;
	if (increment == "null") {
		localStorage.setItem('increment', 1);
		newNum = localStorage.getItem('increment');
	} else {
		newNum = Number(increment) + 1;
		localStorage.setItem('increment', newNum);
	}

	var size = extractNum(filtered[0].in_size);
	var newUnit = "";
	if (size.measurement == ".IN" || size.measurement == "X.IN" || size.measurement == "XIN") {
		newUnit = 'IN';
	} else if (size.measurement == ".IN,LENGTH") {
		newUnit = 'LENGTH';
	}
	else {
		newUnit = size.measurement;
	}

	let remarks = "";
	let holder = filtered[0].in_remarks === null ? "" : filtered[0].in_remarks;
	let remclass = "";
	if (holder.trim() == 'critical') {
		remarks = 'Critical';
		remclass = 'stat-re-order';
	}
	else if (holder.trim() == 'standard') {
		remarks = 'Standard';
		remclass = 'stat-standard';
	}
	else if (holder.trim() == 'average') {
		remarks = 'Average';
		remclass = 'stat-average';
	}
	var row = `<tr id="${filtered[0].in_code}">`;
	row += `<td><label>${filtered[0].in_name}</label></td>`;
	row += `<td><label>${filtered[0].in_category}</label></td>`;
	row += `<td><label>${filtered[0].in_type}</label></td>`;
	row += `<td><label>${filtered[0].in_size}</label></td>`;
	row += `<td><label class="${remclass}" style="font-weight:bold;">${remarks}</label></td>`;
	row += `<td><label>${quantity + " " + newUnit}</label></td>`;
	row += `<td><a style="text-decoration:underline;" href="#" id="delBtn${newNum}" class="delete-row">Delete</a></td>`;
	row += `</tr>`;
	crtable.innerHTML += row;
}

function setTable(array) {
	table.innerHTML = '';
	var emp_role = localStorage.getItem("emp_role");
	var className = "";
	var button;
	if (array.length != 0) {
		for (var i = 0; i < array.length; i++) {
			

				if (array[i].in_category == null) {
					array[i].in_category = '';
				}
				if (array[i].in_type == null) {
					array[i].in_type = '';
				}
				if (array[i].in_size == null || array[i].in_size == "") {
					array[i].in_size = '';
				}
				let remarks = "";
				let holder = array[i].in_remarks === null ? "" : array[i].in_remarks;
				let remclass = "";
				if (holder.trim() == 'critical') {
					remarks = 'Critical';
					remclass = 'stat-re-order';
					let row = `<tr>`;
					row += `<td id="in_dateAdded"><label>${array[i].in_dateAdded}</label></td>`;
					row += `<td id="in_code"><label>${array[i].in_code}</label></td>`;
					row += `<td name="in_name"><label>${array[i].in_name}</label></td>`;
					row += `<td name ="in_category"> <label>${array[i].in_category}</label></td>`;
					row += `<td name="in_type"><label>${array[i].in_type}</label></td>`;
					row += `<td name="in_size"><label>${array[i].in_size}</label></td>`;
					row += `<td name="in_quantity"><label>${array[i].in_quantity}</label></td>`;
					row += `<td name="in_remarks"><label class="${remclass}" style="color: ${remarks === 'Critical' ? 'red' : 'inherit'}">${remarks}</label></td>`;
					row += `<td name="in_class"><label>${array[i].in_class}</label></td>`;
					row += `</tr>`;
					table.innerHTML += row;
				}
			}
			if (table.innerHTML == '') {
				let errorMessageRow = document.createElement('tr');
				errorMessageRow.style.textAlign = "center";
				errorMessageRow.style.fontStyle = "italic";
				errorMessageRow.innerHTML = "<td colspan='9'>No items found<td>";
				table.appendChild(errorMessageRow);
			}
		}
    else {
			let errorMessageRow = document.createElement('tr');
			errorMessageRow.style.textAlign = "center";
			errorMessageRow.style.fontStyle = "italic";
			errorMessageRow.innerHTML = "<td colspan='9'>No items found<td>";
			table.appendChild(errorMessageRow);
		}
	}

function fixArray(array, boolean) {
	if (boolean == 1) {
		for (var j = 0; j < array[0].length; j++) {
			allRequests.push(array[0][j]);
			fixedArray.push(array[0][j]);
		}
	}
	else if (boolean == 2) {
		for (var j = 0; j < array[0].length; j++) {
			newEmployee.push(array[0][j]);
		}
	}
	else {
		for (var j = 0; j < array[0].length; j++) {
			itemInv.push(array[0][j]);
		}
	}
}

//#region table search function
function SearchItem1(value) {
	filterArray1(value.toLowerCase());
	if (value == '') {
		InvsetTable(itemInv);
	} else {
		InvsetTable(filtered);
	}
}
function filterArray1(value) {
	filtered.length = 0;
	for (var j = 0; j < itemInv.length; j++) {
		if (JSON.stringify(itemInv[j]).toLowerCase().includes(value)) {
			filtered.push(itemInv[j]);
		}
	}
}
function SearchItem(value) {
	NewfilterArray(value.toLowerCase());
	if (value == '') {
		setTable(viewData);
	} else {
		setTable(filtered);
	}
}
function filterArray(value) {
	filtered.length = 0;
	for (var j = 0; j < allRequests.length; j++) {
		if (JSON.stringify(allRequests[j]).toLowerCase().includes(value)) {
			filtered.push(allRequests[j]);
		}
	}
}
function FindId(value) {
	filtered.length = 0;
	let arr = [];
	for (var j = 0; j < allRequests.length; j++) {
		if (JSON.stringify(allRequests[j].request_type_id).toLowerCase().includes(value)) {
			arr.push(allRequests[j]);
		}
	}
	return arr;
}
function NewfilterArray(value) {
	//const rows = Array.from(table.getElementsByTagName('tr')).slice(1);
	filtered.length = 0;
	for (var j = 0; j < viewData.length; j++) {
		if (JSON.stringify(viewData[j]).toLowerCase().includes(value)) {
			filtered.push(viewData[j]);
		}
	}
}
//#endregion

function UpdateStatus(idToFind, type) {
	filtered.length = 0;
	var arr = FindId(idToFind);
	let edit = "";
	let requestType = "";
	if (type == 'approve') {
		edit = "Approved";
		requestType = "approved";
		message = "Request approved";
	}
	else {
		edit = "Declined";
		requestType = "declined";
		message = "Request declined";
	}
	arr.forEach((item) => {
		item.request_status = edit;
	});
	sendRequest(arr, requestType)
}

function saveRequest() {
	var body = JSON.parse(localStorage.getItem('bodyData'));
	var type = localStorage.getItem('typeData');
	sendRequest(body, type);
	localStorage.setItem('bodyData', '');
	localStorage.setItem('typeData', '');
}

function sendRequest(jsonData, formType) {
	fetch("/Admin/Requisition?formType=" + formType, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json'
		},
		body: JSON.stringify(jsonData)
	})
		.then(response => {
			if (response.ok) {
				return response.text()
			}
			else {
				throw new Error('Network response was not ok');
			}
		})
		.then(data => {
			var message = JSON.parse(data);
			//if (formType == 'approved') {
			//	message = "Request approved";
			//}
			//else {
			//	message = "Request declined";
			//}
			console.log('Response from server:', data);
			window.location.reload();
			localStorage.setItem("success", message);
		})
		.catch(error => {
			console.error('Error:', error);
		});
}

function setTemplate(Id, typeOf) {
	//var fields = document.querySelectorAll(domId);
	const rows = Array.from(crtable.getElementsByTagName('tr'));
	let emp_no = localStorage.getItem('emp_no');

	var fields = document.querySelectorAll(Id);
	var arr = [];
	var arr2 = [];
	var stats = "";
	if (typeOf == "add") {
		stats = "on process";
	}
	fields.forEach((label) => {
		if (label.getAttribute('id') == 'reqId') {
			arr2.push(extractNum(label.innerText).num);
		}
		if (label.getAttribute('id') == 'reqName') {
			arr2.push(emp_no);
		}
		if (label.getAttribute('id') == 'reqType') {
			arr2.push(label.innerText);
		}
		if (label.getAttribute('id') == 'reqDate') {
			arr2.push(label.innerText);
		}
	});
	rows.forEach((row) => {
		let newObj = Object.assign({}, template);
		newObj.request_type_id = arr2[0];
		newObj.request_employee_id = arr2[1];
		newObj.request_type = arr2[2];
		newObj.request_date = arr2[3];
		newObj.request_status = 'on process';
		newObj.request_item = row.getAttribute('id');
		newObj.request_item_quantity = row.cells[5].innerText;
		arr.push(newObj);
	});
	return arr;
}

//#region sort only
function SortDescending(array, itemToSort) {
	return array.sort((a, b) => {
		if (a[itemToSort] < b[itemToSort]) return 1;
		if (a[itemToSort] > b[itemToSort]) return -1;
		return 0;
	});
}
function SortAscending(array, itemToSort) {
	return array.sort((a, b) => {
		if (a[itemToSort] < b[itemToSort]) return -1;
		if (a[itemToSort] > b[itemToSort]) return 1;
		return 0;
	});
}
// sort from columns
function Sort(item, value) {
	let iElement = item.querySelector('i');
	let arrayToSend = [];
	let catcher = [];

	if (filtered.length != 0) {
		arrayToSend = filtered;
	}
	else {
		arrayToSend = viewData;
	}

	if (iElement.classList.contains('la-sort-down')) {
		iElement.classList.remove('la-sort-down');
		iElement.classList.add('la-sort-up');
		catcher = SortAscending(arrayToSend, value);
	} else {
		iElement.classList.remove('la-sort-up');
		iElement.classList.add('la-sort-down');
		catcher = SortDescending(arrayToSend, value);
	}
	setTable(catcher);
}

function SortByCategory(element, value) {
	let focuselement = "";
	if (value != '') {
		filterArray(value.toLowerCase(), fixedArray);
	}
	else {
		filterArray("", fixedArray);
		element.selectedIndex = 0;
	}
	setTable(filtered);
}
//#endregion

//#region request details
function MaxRequestId() {
	const requestTypeIds = viewData.map(request => request.request_type_id);
	const maxRequestTypeId = Math.max(...requestTypeIds);
	return maxRequestTypeId;
}
function SetLabels() {
	let reqId = document.getElementById('reqId');
	let reqDate = document.getElementById('reqDate');
	let reqName = document.getElementById('reqName');
	let reqType = document.getElementById('reqType');
	let type = "";
	let type1 = "";

	if (emp_role == 2) {
		type = "PUR";
		type1 = "Purchase";
	}
	else if (emp_role == 3) {
		type = "SUP";
		type1 = "Supply";
	}
	else {
		type = "DEP";
		type1 = "Deployment";
	}
	let maxNum = Number(MaxRequestId()) + Number(1);
	reqId.innerText = `${type}${maxNum}`;
	reqDate.innerText = getDateNow();
	reqType.innerText = type1;
	reqName.innerText = emp_name;
}

function getDateNow() {
	const now = new Date();
	const options = { year: 'numeric', month: 'long', day: 'numeric' };
	const formattedDate = now.toLocaleDateString('en-US', options);
	return formattedDate;
}
//#endregion
