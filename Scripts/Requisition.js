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
let crtable = document.querySelector('#create-table tbody');
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');
let addItem = document.getElementById('add-item');
let table = document.querySelector('#myTable tbody');
let emp_role = localStorage.getItem('emp_role');
let reqDetails = document.querySelector('#reqDetails tbody');
let apprItems = document.querySelector('#apprItems tbody');
let reqId = document.getElementById('edit-reqId');
let reqDate = document.getElementById('edit-reqDate');
let reqName = document.getElementById('edit-reqName');
let reqType = document.getElementById('edit-reqType');
let emp_no = localStorage.getItem('emp_no');
let guids = [];
let message = "";

const template = {
	"request_id": null,
	"request_type": "",
	"request_item": "",
	"request_item_quantity": "",
	"request_date": "",
	"request_employee_id": "",
	"request_status": "",
	"request_type_id": 0,
	"request_type_status": ""
};

const template2 = {
	"request_type": "",
	"request_date": "",
	"request_employee_id": "",
	"request_type_id": 0,
	"request_type_status": ""
};

document.getElementById('saveBtn').addEventListener('click', function () {
	let typeOf = localStorage.getItem('typeOf');
	infoOpenPopup();
	if (typeOf == 'change') {
		saveEditTemplate();
	}
	else {
		EditSetTemplate();
	}
});

addItem.addEventListener("click", function (event) {
	event.preventDefault();
	InvsetTable(itemInv);
});
document.getElementById('create-table').addEventListener("click", function (event) {
	const target = event.target;
	if (target.tagName.toLowerCase() === "a" && target.classList.contains("delete-row")) {
		const row = target.parentNode.parentNode;
		row.onclick = null;
		row.parentNode.removeChild(row);
	}
});
document.getElementById('view-request-popup-pur').addEventListener("click", function (event) {
	const target = event.target;
	if (target.tagName.toLowerCase() === "a" && target.classList.contains("delete-row")) {
		const row = target.parentNode.parentNode;
		row.parentNode.removeChild(row);
	}
});
document.getElementById('cnfBtn').addEventListener("click", function () {
	let body = setTemplate('#create-fields label', 'add');
	localStorage.setItem('bodyData', '');
	localStorage.setItem('typeData', '');
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
			/*window.location.replace('/Error/InternalServerError');*/
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
			/*window.location.replace('/Error/InternalServerError');*/
			console.error(error);
		});
}
function GetAll() {
	fetch('/Admin/FindDataOf?requestType=requisition')
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
			allRequests.length = 0;
			jsonArray.push(data);
			fixArray(jsonArray, 1);
			DeleteCertainPart();
			//if (table !== null) {
			let result = MergeSameId(viewData);
			setTable(result);
			//}
		})
		.catch(error => {
			/*//window.location.replace('/Error/InternalServerError');*/
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
	for (let i = 0; i < allRequests.length; i++) {
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
	for (let i = 0; i < array.length; i++) {
		let remarks = "";
		let holder = array[i].in_remarks === null ? "" : array[i].in_remarks;
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
		let row = `<tr onclick="openInputQTYForm('${array[i].in_code}')">`;
		row += `<td> ${array[i].in_name}</td>`;
		row += `<td><label>${array[i].in_category}</label></td>`;
		row += `<td><label>${array[i].in_type}</label></td>`;
		row += `<td><label>${array[i].in_size}</label></td>`;
		row += `<td><label class="${remclass}" style="font-weight:bold;">${remarks}</label></td>`;
		row += `</tr>`;
		invTable.innerHTML += row;
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

	let size = extractNum(filtered[0].in_quantity);
	let newUnit = "";
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
	let row = `<tr id="${filtered[0].in_code}">`;
	row += `<td><label>${filtered[0].in_name}</label></td>`;
	row += `<td><label>${filtered[0].in_category}</label></td>`;
	row += `<td><label>${filtered[0].in_type}</label></td>`;
	row += `<td><label>${filtered[0].in_size}</label></td>`;
	row += `<td><label class="${remclass}" style="font-weight:bold;">${remarks}</label></td>`;
	row += `<td><label>${quantity + " " + newUnit}</label></td>`;
	//id = "delBtn${newNum}"
	row += `<td><a style="text-decoration:underline;" href="#" class="delete-row">Delete</a></td>`;
	row += `</tr>`;
	crtable.innerHTML += row;
}

function setTable(array) {
	table.innerHTML = '';
	let emp_role = localStorage.getItem("emp_role");
	let className = "";
	let editBtn;
	let apprBtn;
	if (array.length != 0) {
		for (let i = 0; i < array.length; i++) {
			let changeListener = "";
			let editListener = "";

			if (emp_no == array[i].Employee.emp_no) {
				changeListener = `viewrequestOpenPopupPur('${array[i].request_type_id}', 'change')`;
				editListener = "";
			}
			else {
				editListener = `viewrequestOpenPopupPur('${array[i].request_type_id}', 'edit')`;
			}
			if (array[i].request_status.trim().toLowerCase() === "approved") {
				editListener = "";
			}
			if (array[i].request_status === "On Process") {
				className = "stat-pen";
			}
			if (array[i].request_status === "Pending") {
				className = "stat-dec";
			}
			if (array[i].request_status === "Approved") {
				className = "stat-app";
			}

			//<a onclick="viewrequestOpenPopupPur()"></a>
			let row = `<tr id="${array[i].request_type_id}">`;
			row += `<td class="view" onclick="viewrequestOpenPopupPur()" id="${array[i].request_id}"> ${array[i].Id}</td>`;
			row += `<td onclick="viewrequestOpenPopupPur(${array[i].request_type_id},'view')"><label>${array[i].request_type}</label></td>`;
			row += `<td onclick="viewrequestOpenPopupPur(${array[i].request_type_id},'view')"><label>${array[i].Employee.emp_fname + " " + array[i].Employee.emp_lname}</label></td>`;
			row += `<td onclick="viewrequestOpenPopupPur(${array[i].request_type_id},'view')"><label>${array[i].Employee.emp_no}</label></td>`;
			row += `<td onclick="viewrequestOpenPopupPur(${array[i].request_type_id},'view')"><label>${array[i].FormattedDate}</label></td>`;
			row += `<td onclick="viewrequestOpenPopupPur(${array[i].request_type_id},'view')"><label class="${className}" style="font-weight:bold;">${array[i].request_status}</label></td>`;
			row += `<td id="hideActionBtn">`;
			row += `<div class="purchase-action-style">`;
			row += `<button class="acc-btn" id="appr${i}" title="ACCEPT REQUEST" onclick="${editListener}"> <a href="#"><span class="las la-check-circle"></span></a></button>`;
			//row += `<button class="dec-btn" id="dec${i}" title="DECLINE REQUEST" onclick="UpdateStatus('${array[i].request_type_id}','pending')"> <a href="#"><span class="las la-times-circle"></span></a></button>`;
			row += `<button class="edit-btn" title="EDIT SELECTED ITEM" id="edit${i}" onclick="${changeListener}"><span class="lar la-edit"></span></button>`;

			row += `<button class="expo-btn" onclick = "window.location.href='/admin/ExportReportNew?id=${array[i].request_type_id}'" title="EXPORT REPORT"><span class="las la-file-download"></span></button>`;
			
			row += `</div></td>`;
			row += `</tr>`;
			table.innerHTML += row;

			//if (array[i].request_status === "Declined") {
			//	button = document.getElementById(`dec${i}`);
			//	button.disabled = true;
			//	button.style.cursor = "not-allowed";
			//	button.style.opacity = 0.5;
			//}
			//if (array[i].request_status === "Approved") {
			//	button = document.getElementById(`appr${i}`);
			//	button.disabled = true;
			//	button.style.cursor = "not-allowed";
			//	button.style.opacity = 0.5;
			//}
			if (array[i].request_status.trim().toLowerCase() === "approved") {
				apprBtn = document.getElementById(`appr${i}`);
				apprBtn.disable = true;
				apprBtn.style.cursor = "not-allowed";
				apprBtn.style.opacity = 0.5;
				editBtn = document.getElementById(`edit${i}`);
				editBtn.disable = true;
				editBtn.style.cursor = "not-allowed";
				editBtn.style.opacity = 0.5;
			}
			if (emp_no != array[i].Employee.emp_no) {
				editBtn = document.getElementById(`edit${i}`);
				editBtn.disable = true;
				editBtn.style.cursor = "not-allowed";
				editBtn.style.opacity = 0.5;
			}
			else {
				apprBtn = document.getElementById(`appr${i}`);
				apprBtn.disable = true;
				apprBtn.style.cursor = "not-allowed";
				apprBtn.style.opacity = 0.5;
			}
		}
		filtered.length = 0;
	}
	else {
		//error handler if input value not found
		table.innerHTML = " ";
		const errorMessageRow = document.createElement('tr');
		errorMessageRow.style.textAlign = "center";
		errorMessageRow.style.fontStyle = "italic";
		errorMessageRow.innerHTML = "<td colspan='8'>Item Not found<td>";
		//console.log(res.statusText);
		table.appendChild(errorMessageRow);
	}
}

function fixArray(array, boolean) {
	if (boolean == 1) {
		for (let j = 0; j < array[0].length; j++) {
			allRequests.push(array[0][j]);
			fixedArray.push(array[0][j]);
		}
		if (emp_role == 2) {
			//allRequests = allRequests.filter(obj => obj.request_type !== 'Supply' && obj.request_status !== 'Approved');
			let tmp = [];
			for (let i = 0; i < allRequests.length; i++) {
				if (allRequests[i].request_type === 'Supply') {
					if (allRequests[i].request_status === 'Approved') {
						tmp.push(allRequests[i]);
					}
				}
				else {
					tmp.push(allRequests[i]);
				}
			}
			allRequests.length = 0;
			allRequests = tmp;
		}
	}
	else if (boolean == 2) {
		for (let j = 0; j < array[0].length; j++) {
			newEmployee.push(array[0][j]);
		}
	}
	else {
		for (let j = 0; j < array[0].length; j++) {
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
//Inventory
function filterArray1(value) {
	filtered.length = 0;
	for (let j = 0; j < itemInv.length; j++) {
		if (JSON.stringify(itemInv[j]).toLowerCase().includes(value)) {
			filtered.push(itemInv[j]);
		}
	}
}
//Employee
function filterArray2(value) {
	filtered.length = 0;
	for (let j = 0; j < newEmployee.length; j++) {
		if (JSON.stringify(newEmployee[j]).toLowerCase().includes(value)) {
			filtered.push(newEmployee[j]);
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
	for (let j = 0; j < allRequests.length; j++) {
		if (JSON.stringify(allRequests[j]).toLowerCase().includes(value)) {
			filtered.push(allRequests[j]);
		}
	}
}
function FindId(value) {
	filtered.length = 0;
	let arr = [];
	for (let j = 0; j < allRequests.length; j++) {
		if (JSON.stringify(allRequests[j].request_type_id).toLowerCase().includes(value)) {
			arr.push(allRequests[j]);
		}
	}
	return arr;
}
function NewfilterArray(value) {
	//const rows = Array.from(table.getElementsByTagName('tr')).slice(1);
	filtered.length = 0;
	for (let j = 0; j < viewData.length; j++) {
		if (JSON.stringify(viewData[j]).toLowerCase().includes(value)) {
			filtered.push(viewData[j]);
		}
	}
}
//#endregion

//#region approved/declined functions for checking
function UpdateStatus(idToFind, type) {
	filtered.length = 0;
	let arr = FindId(idToFind);
	let edit = "";
	let typestats = "";
	let requestType = "";
	if (type == 'approve') {
		edit = "Approved";
		requestType = "approved";
		typestats = "Approved";
		message = "Request approved";
	}
	else {
		edit = "Pending";
		requestType = "declined";
		message = "Request declined";
	}
	arr.forEach((item) => {
		item.request_status = edit;
		if (typestats !== "") {
			item.request_type_status = typestats;
		}
		else {
			item.request_type_status = 'Active';
		}
	});
	sendRequest(arr, requestType)
}

function saveRequest() {
	let body = JSON.parse(localStorage.getItem('bodyData'));
	let type = localStorage.getItem('typeData');
	sendRequest(body, type);
	localStorage.setItem('bodyData', '');
	localStorage.setItem('typeData', '');
}

function setTemplate(Id, typeOf) {
	//let fields = document.querySelectorAll(domId);
	//let emp_no = localStorage.getItem('emp_no');

	let fields = document.querySelectorAll(Id);
	let arr = [];
	let arr2 = [];
	let stats = "";
	let cell = 0;
	if (typeOf == "add") {
		const rows = Array.from(crtable.getElementsByTagName('tr'));
		cell = 5;
		stats = "on process";
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
			newObj.request_status = stats;
			newObj.request_item = row.getAttribute('id');
			newObj.request_item_quantity = row.cells[cell].innerText;
			newObj.request_type_status = 'Active';
			arr.push(newObj);
		});
	}
	else if (typeOf == 'edit') {
		// TO DO
	}

	return arr;
}
//#endregion

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
			let message = JSON.parse(data);
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
// on create request
function SetLabels() {
	let reqId = document.getElementById('reqId');
	let reqDate = document.getElementById('reqDate');
	let reqName = document.getElementById('reqName');
	let reqType = document.getElementById('reqType');
	let emp_name = localStorage.getItem('emp_name');

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

//#region set template on "check" icon
function EditSetTemplate() {
	localStorage.setItem('bodyData', '');
	localStorage.setItem('typeData', '');
	let reqs = reqDetails.querySelectorAll('tr');
	let apprs = apprItems.querySelectorAll('tr');
	let newReqs = JSON.parse(localStorage.getItem("newReqsData"));
	let arr = [];
	//let range = Number(reqs.length) + Number(apprs.length);

	newReqs.forEach((row) => {
		let newObj = Object.assign({}, template);
		newObj.request_id = row.request_id;
		newObj.request_type_id = row.request_type_id;
		newObj.request_employee_id = row.request_employee_id;
		newObj.request_type = row.request_type;
		newObj.request_date = row.request_date;
		if (reqs.length === 0) {
			newObj.request_status = "Approved";
		}
		else {
			newObj.request_status = "Pending";
		}
		newObj.request_item = row.request_item;
		newObj.request_item_quantity = row.request_item_quantity;
		//newObj.request_type_status = "Active";
		if (reqs.length != 0) {
			reqs.forEach((item) => {
				if (item.getAttribute('id') == row.request_item) {
					newObj.request_type_status = "Active";
				}
			});
		}
		if (apprs.length != 0) {
			apprs.forEach((item) => {
				if (item.getAttribute('id') == row.request_item) {
					newObj.request_type_status = "Approved";
				}
			});
		}
		arr.push(newObj);
	});
	localStorage.setItem('bodyData', JSON.stringify(arr));
	localStorage.setItem('typeData', 'edit');
}
//#endregion

//#region display data when row is clicked
function DisplayDataOnRow(idToFind, typeOf) {
	localStorage.setItem('typeOf', typeOf);
	reqDetails.innerHTML = "";
	apprItems.innerHTML = "";
	let emps = [];
	let newReqs = [];
	let items = [];
	let row = "";

	filterArray(idToFind);
	newReqs = [...filtered];
	filterArray2(newReqs[0].request_employee_id);
	emps = [...filtered];
	localStorage.setItem("newReqsData", JSON.stringify(newReqs));
	//localStorage.setItem("req_status", newReqs[0].request_status);

	for (let i = 0; i < newReqs.length; i++) {
		for (let j = 0; j < itemInv.length; j++) {
			if (newReqs[i].request_item == itemInv[j].in_code) {
				items.push(itemInv[j]);
				guids.push(newReqs[i].request_id);
			}
		}
	}
	reqId.innerText = newReqs[0].Id;
	localStorage.setItem('stats', newReqs[0].request_status);
	reqDate.innerText = newReqs[0].FormattedDate;
	reqName.innerText = emps[0].emp_fname + " " + emps[0].emp_lname;
	reqType.innerText = newReqs[0].request_type;

	for (let ind = 0; ind < newReqs.length; ind++) {
		row = '';
		let listener = "";
		let className = "";
		if (typeOf === "edit") {
			listener = `onclick="SeperateRow(this)"`;
		}
		else if (typeOf === "change") {
			listener = `onclick="openInputQTYForm1(this)"`;
		}
		else {
			listener = ``;
		}

		if (newReqs[ind].request_type_status.trim() == 'Active') {
			className = 'stat-active';
		}
		else {
			className = 'stat-approved';
		}

		row += `<tr ${listener} class="view" id="${items[ind].in_code}">`
		row += `<td class="checkbox-view edit-td"><input type="checkbox"></td>`;
		row += `<td id="${guids[ind]}">${items[ind].in_name}</td>`;
		row += `<td >${items[ind].in_category}</td>`;
		row += `<td >${items[ind].in_type}</td>`;
		row += `<td >${items[ind].in_size}</td>`;
		row += `<td >${items[ind].in_quantity}</td>`;
		row += `<td >${newReqs[ind].request_item_quantity}</td>`;
		row += `<td class="${className}">${newReqs[ind].request_type_status}</td>`;
		if (typeOf === "change") {
			row += `<td><a style="text-decoration:underline;" href="#" id="delBtn${ind}" class="delete-row">Delete</a></td>`;
		}
		row += `</tr>`;
		if (newReqs[ind].request_type_status.trim() == 'Active' || newReqs[ind].request_type_status == null) {
			reqDetails.innerHTML += row;
		}
		else {
			apprItems.innerHTML += row;
		}
	}

	let edittd = document.querySelectorAll(".edit-td");
	let closeBtn = document.getElementById("closeBtn");
	if (typeOf == 'edit') {
		edittd.forEach((td) => {
			td.classList.remove('checkbox-view');
		});
		closeBtn.style.display = "none";
	}
	else if (typeOf == 'change') {
		closeBtn.style.display = "none";
		let btns = document.getElementById('frmBtn');
		btns.classList.remove('checkbox-view');
	}
	else {
		edittd.forEach((td) => {
			td.classList.add('checkbox-view');
		});
		closeBtn.style.display = "inline-flex";
	}

	let rows1 = apprItems.querySelectorAll('.edit-td');
	if (typeOf == "change") {
		document.getElementById("actCol").style.display = "table-cell";
		document.getElementById("actCol1").style.display = "table-cell";
	}
	else {
		document.getElementById("actCol").style.display = "none";
		document.getElementById("actCol1").style.display = "none";
	}
	rows1.forEach((row) => {
		let checkbox = row.querySelector('input[type="checkbox"]');
		checkbox.checked = true;
	});
}

function SeperateRow(row) {
	let checkbox = row.querySelector('input[type="checkbox"]');

	if (!checkbox.checked) {
		checkbox.checked = true;
		apprItems.appendChild(row);
		row.cells[7].innerText = "Approved";
		row.cells[7].classList.remove('stat-active');
		row.cells[7].classList.add('stat-approved');
	}
	else {
		checkbox.checked = false;
		reqDetails.appendChild(row);
		row.cells[7].innerText = "Active";
		row.cells[7].classList.remove('stat-approved');
		row.cells[7].classList.add('stat-active');
	}
}
//#endregion

//#region set template for edit button

function saveEditTemplate() {

	let reqTr = reqDetails.querySelectorAll('tr');
	let apprTr = apprItems.querySelectorAll('tr');
	let prod = localStorage.getItem('stats');
	let newReqs = JSON.parse(localStorage.getItem("newReqsData"));
	let arr = [];

	if (reqTr.length != 0) {
		reqTr.forEach((row) => {
			let newObj = Object.assign({}, template);
			newObj.request_id = row.cells[1].getAttribute('id');
			newObj.request_type_id = reqId.innerText;
			newObj.request_employee_id = reqName.innerText;
			newObj.request_type = reqType.innerText;
			newObj.request_date = reqDate.innerText;
			newObj.request_status = prod;
			newObj.request_item = row.getAttribute('id');
			newObj.request_item_quantity = row.cells[6].innerText;
			newObj.request_type_status = row.cells[7].innerText;
			arr.push(newObj);
		});
	}
	if (apprTr.length != 0) {
		apprTr.forEach((tr) => {                                                                                                                                                                  
			let newObj = Object.assign({}, template);
			newObj.request_id = row.cells[1].getAttribute('id');
			newObj.request_type_id = reqId.innerText;
			newObj.request_employee_id = reqName.innerText;
			newObj.request_type = reqType.innerText;
			newObj.request_date = reqDate.innerText;
			newObj.request_status = prod;
			newObj.request_item = row.getAttribute('id');
			newObj.request_item_quantity = row.cells[6].innerText;
			newObj.request_type_status = row.cells[7].innerText;
			arr.push(newObj);
		});
	}
	console.log(arr);
	localStorage.setItem('bodyData', '');
	localStorage.setItem('typeData', '');
	localStorage.setItem('bodyData', JSON.stringify(arr));
	localStorage.setItem('typeData', 'edit');
}

//#endregion