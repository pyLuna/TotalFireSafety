	let jsonArray = [];
let filtered = [];
let itemCategories = [];
let newArray = [];
let fixedArray = [];
let formsId = ['#qtyForm', '#addForm', '#editForm'];
let sliced = "";
let prevVal = "";
let prevNum = "";
let table = document.querySelector('#myTable tbody');
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');
//#region add form elements
let category = document.getElementById('in_category');
let class1 = document.getElementById('class');
let class2 = document.getElementById('in_class');
let name = document.getElementById('in_name');
let type = document.getElementById('in_type');
let itemCode = document.getElementById('in_code');
let dateAdded = document.getElementById('dateAdded');
let add = document.querySelectorAll('#addForm');
let addForm = document.querySelectorAll('#addForm datalist');
let addFormSel = document.querySelectorAll('#addForm select');
let formBtns = document.querySelectorAll('#addForm .form-add-btns button');
//#endregion

let selcat3 = document.getElementById('selcat2');
let name3 = document.getElementById('in_name2');
let selclass3 = document.getElementById('selclass2');
let in_code3 = document.getElementById('in_code2');
let formType3 = document.getElementById('formType2');

let qtyCode = document.getElementById('in_code1');
let qtyFormSel = document.querySelectorAll('#qtyForm select');
let qty = document.querySelectorAll('#qtyForm');
let selcat1 = document.getElementById('selcat1');
let selclass = document.getElementById('selclass');
let seltype = document.getElementById('seltype');
let inpSize1 = document.getElementById('inpSize1');
let inpSize = document.getElementById('inpSize');
let inpQuant = document.getElementById('inpQuant');
let inpQuant1 = document.getElementById('inpQuant1');

let editFormSel = document.querySelectorAll('#editForm select');

let edit = document.querySelectorAll('#editForm');
let imgContainer = document.getElementById("image-container");
let EditFormBtns = document.querySelectorAll('#editForm .form-add-btns button');
let QtyFormBtns = document.querySelectorAll('#qtyForm .form-add-btns button');
// laging null lumalabas dito
let sessionId = localStorage.getItem("emp_role");

//#region Listener area
class1.addEventListener("change", SetItemCode);
category.addEventListener("input", SetItemCode);
name.addEventListener("input", SetItemCode);
formBtns.forEach((btns) => {
	btns.addEventListener("click", function (event) {
		event.preventDefault();
	});
});
EditFormBtns.forEach((btns) => {
	btns.addEventListener("click", function (event) {
		event.preventDefault();
	});
});
QtyFormBtns.forEach((btns) => {
	btns.addEventListener("click", function (event) {
		event.preventDefault();
	});
});
qtyCode.addEventListener("keydown",Scan)
//#endregion

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
				errorMessageRow.innerHTML = '<td colspan="9">Loading Error<td>';
				table.appendChild(errorMessageRow);
			}
		})
		.then(data => {
			jsonArray.push(data);
			fixArray();
			if (table !== null) {
				setTable(fixedArray);
			}
		})
		.catch(error => {
			//window.location.replace('/Error/InternalServerError');
			console.error(error);
		});
}
function DeleteItem() {
	let item = localStorage.getItem("code");

	fetch('/Admin/DeleteItem?item=' + item, {
		method: "POST"
	})
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				console.log(res.statusText);
			}
		})
		.then(data => {
			localStorage.setItem("removed", "success");
			window.location.reload();

		})
		.catch(error => {
			console.error(error);
		});
}
function GetBarcode() {
	event.preventDefault();
	fetch('/Admin/GetBarcode?itemCode=' + itemCode.value, {
		method: "POST"
	})
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				console.log(res.statusText);
			}
		})
		.then(data => {
			const img = document.createElement('img');
			// find the container element to append the image to
			var parsed = JSON.parse(data);
			// set its source to the image data
			img.setAttribute("id", "barcodeImg");
			//img.setAttribute("download", "filename.png");
			img.src = `data:image/pmg;base64,${parsed}`;

			//const container = document.getElementById('image-container');

			// append the image to the container element
			imgContainer.innerHTML = "";
			imgContainer.appendChild(img);
		})
		.catch(error => {
			imgContainer.style.display = "none";
			//console.error(error);
		});
}

//#region Others
function fixArray() {
	for (let j = 0; j < jsonArray[0].length; j++) {
		fixedArray.push(jsonArray[0][j]);
	}
}

function setTable(array) {
	table.innerHTML = '';
	if (array.length != 0) {
		for (let i = 0; i < array.length; i++) {

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
			}
			else if (holder.trim() == 'standard') {
				remarks = 'Standard';
				remclass = 'stat-standard';
			}
			else if (holder.trim() == 'average'){
				remarks = 'Average';
				remclass = 'stat-average';
			}
			/* class ng remarks 
			 * stat-standard
			 * stat-average
			 * stat-re-order
			 */
			let row = `<tr>`; /*onclick = "canOpenPopup()"*/
			row += `<td id="in_code"><label>${array[i].in_code}</label></td>`;
			row += `<td name="in_name"><label>${array[i].in_name}</label></td>`;
			row += `<td name ="in_category"> <label>${array[i].in_category}</label></td>`;
			row += `<td name="in_type"><label>${array[i].in_type}</label></td>`;
			row += `<td name="in_size"><label>${array[i].in_size}</label></td>`;
			row += `<td name="in_quantity"><label>${array[i].in_quantity}</label></td>`;
			row += `<td name="in_remarks"><label class="${remclass}">${remarks}</label></td>`;
			row += `<td name="in_class"><label>${array[i].in_class}</label></td>`;
			if (sessionId == 2) {
			row += `<td id="hideActionBtn"><div class="inventory-action-style">`;
			row += `<button class="edit-btn" title="EDIT SELECTED ITEM" onclick="OpenEdit('${array[i].in_code}')"> <a href="#"><span class="lar la-edit"></span></a></button>`;
			row += `<button class="del-btn" title="DELETE SELECTED ITEM" onclick = "delOpenPopup('${array[i].in_code}')"> <a href="#"><span class="lar la-trash-alt"></span></a></button>`;
			row += `</div></td>`;
			}
			row += `</tr>`;
			table.innerHTML += row;
		}
	}
	else {
		//error handler if input value not found
		table.innerHTML = " ";
		let errorMessageRow = document.createElement('tr');
		errorMessageRow.style.textAlign = "center";
		errorMessageRow.style.fontStyle = "italic";
		errorMessageRow.innerHTML = "<td colspan='9'>Item Not found<td>";
		//console.log(res.statusText);
		table.appendChild(errorMessageRow);
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

function getDateNow() {
	const now = new Date();
	const options = { year: 'numeric', month: 'long', day: 'numeric' };
	const formattedDate = now.toLocaleDateString('en-US', options);
	return formattedDate;
}

function resetForm() {
	formsId.forEach(id => {
		let allInputs = `${id} input`;
		let allSelects = `${id} select`;
		let inputs = document.querySelectorAll(allInputs);
		let sels = document.querySelectorAll(allSelects);
		inputs.forEach(item => {
			item.value = "";
		});
		sels.forEach(item => {
			item.selectedIndex = 0;
		});
	});
}
//#endregion

//#region Sort By Column
// sort function start
// General sort function
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
function Sort(item,value) {
	let iElement = item.querySelector('i');
	let arrayToSend = [];
	let catcher = [];

	if (filtered.length != 0) {
		arrayToSend = filtered;
	}
	else {
		arrayToSend = fixedArray;
	}

	if (iElement.classList.contains('la-sort-down')) {
		iElement.classList.remove('la-sort-down');
		iElement.classList.add('la-sort-up');
		catcher = SortAscending(arrayToSend,value);
	} else {
		iElement.classList.remove('la-sort-up');
		iElement.classList.add('la-sort-down');
		catcher = SortDescending(arrayToSend,value);
	}
	setTable(catcher);
}
//sort by category
function SortByCategory(element, value) {
	let focuselement = "";
	if (value != '') {
		FilterItem(value.toLowerCase(), fixedArray);
	}
	else {
		FilterItem("", fixedArray);
		element.selectedIndex = 0;
	}
		setTable(filtered);
}
//#endregion

//#region Search Item
function filterArray(value) {
	filtered.length = 0;
	for (let j = 0; j < fixedArray.length; j++) {
		if (JSON.stringify(fixedArray[j]).toLowerCase().includes(value)) {
			filtered.push(fixedArray[j]);
		}
	}
}

function SearchItem(value) {
	filterArray(value.toLowerCase());
	if (value == '') {
		setTable(fixedArray);
	}
	else {
		setTable(filtered);
	}
}
// wag galawin
function FilterItem(value,array) {
	filtered.length = 0;
	let catvalue = selcat.options[selcat.selectedIndex].value.toLocaleLowerCase();
	let filvalue = filter.options[filter.selectedIndex].value.toLocaleLowerCase();
	for (let j = 0; j < array.length; j++) {
		if (filter.selectedIndex != 0 && selcat.selectedIndex != 0) {
			if (JSON.stringify(array[j].in_category).toLowerCase().includes(catvalue) && JSON.stringify(array[j].in_remarks).toLowerCase().includes(filvalue)) {
				filtered.push(array[j]);
			}
		}
		else {
			if (JSON.stringify(array[j].in_category).toLowerCase().includes(value) || JSON.stringify(array[j].in_remarks).toLowerCase().includes(value)) {
				filtered.push(array[j]);
			}
		}
	}
}
//#endregion

//#region set the attributes of the new item form
function setDatalist() {
	addForm.forEach((datalist) => {
		newArray.length = 0;
		var id = datalist.id;
		var sel = document.getElementById(id) === null ? undefined : document.getElementById(id);

		fixedArray.forEach((item) => {
			if (datalist.id == 'catList') {
				newArray.push(item.in_category);
			}
			else {
				newArray.push(item.in_type);
			}
		});

		var uniques = new Set(newArray);
		datalist.innerHTML = "";
		uniques.forEach((item) => {
			let option = document.createElement('option');
			option.value = item;
			sel.appendChild(option);
		});
	});
}

function AppendOption(array) {
	array.forEach((item) => {
		var optionValue = '';
		newArray.length = 0;
		var id = item.getAttribute('id');
		var sel = document.getElementById(id) === null ? undefined : document.getElementById(id);
		sel.innerHTML = '';
		fixedArray.forEach(function (item) {
			if (id == 'selcat1' || id == 'selcat2') {
				optionValue = 'Select Category';
				newArray.push(item.in_category);
			}
			if (id == 'seltype' || id == 'seltype2') {
				optionValue = 'Select Type';
				newArray.push(item.in_type);
			}
			if (id == 'class' || id == 'selclass' || id == 'selclass2') {
				optionValue = 'Select Class';
				newArray.push(item.in_class);
			}
			if (id == 'sizeSel' || id == 'inpSize' || id == 'inpSize2A') {
				optionValue = 'Unit';
				let sizeResult = extractNum(item.in_size);
				newArray.push(sizeResult.measurement);
			}
			if (id == 'sizeQuant' || id == 'inpQuant' || id == 'inpQuant2A') {
				optionValue = 'Unit';
				let quantResult = extractNum(item.in_quantity);
				newArray.push(quantResult.measurement);
			}
		});

		var option = `<option value="">${optionValue}</option>`;
		var uniques = new Set(newArray);
		uniques.forEach(function (item1) {
			if (item1 == ".IN" || item1 == "X.IN" || item1 == "XIN") {
				item1 = '';
			}
			if (item1 == "LENGTH") {
				item1 = '';
			}
			if (item1 == ".IN,LENGTH") {
				item1 = 'LENGTH';
			}
			if (item1 != "") {
				option += `<option value="${item1}">${item1}</option>`;
			}
		});
		sel.innerHTML = option;
		sel.selectedIndex = 0;
	});
	//sabit lang to para magka value yung date added na input
	dateAdded.value = getDateNow();
}
//#endregion

//#region set item code
function SetItemCode1() {
	formType3.value = in_code3.value;
	var catVal = selcat3.value === '' ? '' : selcat3.value;
	var classVal = selclass3.options[selclass3.selectedIndex].value === '' ? '' : selclass3.options[selclass3.selectedIndex].value;
	var nameVal = name3.value === '' ? '' : name3.value;
	var itemNum = '';
	//var code = itemCode.value;
	//class2.value = classVal;
	var classabbr = abbreviateString(classVal);
	var categoryabbr = abbreviateString(catVal);
	var nameabbr = abbreviateString(nameVal);
	prevVal = in_code3.value;
	var string = `${classabbr}-${categoryabbr}-${nameabbr}${itemNum}`;
	if (catVal !== "" && classVal !== "" && nameVal !== "") {
		in_code3.value = string;
		GetBarcode();
	}
	else {
		in_code3.value = '';
		document.getElementById('barcodeImg').remove();
	}
}
function SetItemCode() {
	var catVal = category.value === '' ? '': category.value  ;
	var classVal = class1.options[class1.selectedIndex].value === '' ? '': class1.options[class1.selectedIndex].value  ;
	var nameVal = name.value === '' ? '' : name.value;
	var itemNum = '';
	//var code = itemCode.value;
	class2.value = classVal;
	var classabbr = abbreviateString(classVal);
	var categoryabbr = abbreviateString(catVal);
	var nameabbr = abbreviateString(nameVal);
	prevVal = itemCode.value;
	var string = `${classabbr}-${categoryabbr}-${nameabbr}${itemNum}`;
	if (catVal !== "" && classVal !== "" && nameVal !== "") {
		itemCode.value = string;
		GetBarcode();
	}
	else {
		itemCode.value = '';
		document.getElementById('barcodeImg').remove();
	}
}
function abbreviateString(str) {
	if (!str) {
		return null;
	}

	var abbreviation = '';
	var consonants = 0;

	for (var i = 0; i < str.length; i++) {
		var char = str.charAt(i).toUpperCase();
		if (/[BCDFGHJKLMNPQRSTVWXYZ]/.test(char)) {
			abbreviation += char;
			consonants++;
			if (consonants === 4) {
				break;
			}
		}
	}

	return abbreviation;
}
//#endregion

//#region insert data on all hidden inputs
//for addForm
function SetHiddenValues() {
	var in_class = document.getElementById('in_class');
	var in_size = document.getElementById('in_size');
	var in_quantity = document.getElementById('in_quantity');
	var in_remarks = document.getElementById('in_remarks');
	var inpSizeSel = document.getElementById('inpSizeSel');
	var sizeSel = document.getElementById('sizeSel');
	var inpSizeQuant = document.getElementById('inpSizeQuant');
	var sizeQuant = document.getElementById('sizeQuant');

	in_class.value = class1.options[class1.selectedIndex].value;
	in_size.value = inpSizeSel.value + ' ' + sizeSel.options[sizeSel.selectedIndex].value;
	in_quantity.value = inpSizeQuant.value + ' ' + sizeQuant.options[sizeQuant.selectedIndex].value;

	if (inpSizeSel > 60) {
		in_remarks.value = 'standard';
	}
	else if (inpSizeSel > 40 && inpSizeSel < 60) {
		in_remarks.value = 'average';
	}
	else {
		in_remarks.value = 'critical';
	}
}

//for edit form
function setHiddensEdit() {
	var catsel = document.getElementById('selcat2');
	var typesel = document.getElementById('seltype2');
	var classsel = document.getElementById('selclass2');
	var inpSize2A = document.getElementById('inpSize2A');
	var inpQuant2A = document.getElementById('inpQuant2A');
	var inpQuant2 = document.getElementById('inpQuant2');
	var inpSize2 = document.getElementById('inpSize2');
	var cat1 = document.getElementById('in_category2');
	var cl1 = document.getElementById('in_class2');
	var ty1 = document.getElementById('in_type2');
	var sz1 = document.getElementById('in_size2');
	var qt1 = document.getElementById('in_quantity2');
	var rm1 = document.getElementById('in_remarks2');
	var oldqt = extractNum(filtered[0].in_quantity);
	var newqt = Number(oldqt.num) + Number(inpQuant2.value);
	cat1.value = catsel.options[catsel.selectedIndex].value;
	cl1.value = classsel.options[classsel.selectedIndex].value;
	ty1.value = typesel.options[typesel.selectedIndex].value;
	qt1.value = newqt + ' ' + inpQuant2A.options[inpQuant2A.selectedIndex].value;
	sz1.value = inpSize2.value + ' ' + inpSize2A.options[inpSize2A.selectedIndex].value;

	if (newqt < 40) {
		rm1.value = 'critical';
	}
	else if (newqt > 60) {
		rm1.value = 'standard';
	}
	else {
		rm1.value = 'average';
	}

	console.log(`${cat1.value}	${cl1.value}	${ty1.value}	${qt1.value}	${sz1.value}`);
}

//for add qty form
function setHiddens() {
	var cat1 = document.getElementById('in_category1');
	var cl1 = document.getElementById('in_class1');
	var ty1 = document.getElementById('in_type1');
	var sz1 = document.getElementById('in_size1');
	var qt1 = document.getElementById('in_quantity1');
	var rm1 = document.getElementById('in_remarks1');
	var oldqt = extractNum(filtered[0].in_quantity);
	var newqt = Number(oldqt.num) + Number(inpQuant1.value);
	cat1.value = selcat1.options[selcat1.selectedIndex].value;
	cl1.value = selclass.options[selclass.selectedIndex].value;
	ty1.value = seltype.options[seltype.selectedIndex].value;
	qt1.value = newqt + ' ' + inpQuant.options[inpQuant.selectedIndex].value;
	sz1.value = inpSize1.value + ' ' + inpSize.options[inpSize.selectedIndex].value;

	if (newqt < 40) {
		rm1.value = 'critical';
	}
	else if (newqt > 60) {
		rm1.value = 'standard';
	}
	else {
		rm1.value = 'average';
	}

	console.log(`${cat1.value}	${cl1.value}	${ty1.value}	${qt1.value}	${sz1.value}`);
}
//#endregion

//#region process barcode
function Scan(event) {
	if (event.which === 13 || event.keyCode === 13) {
		event.preventDefault();
		// retrieve the scanned value
		if (sliced.length == 0) {
			sliced = qtyCode.value;
		}

		if (qtyCode.value.length != prevVal.length) {
			if (prevVal.length != 0) {
				var index = 0;
				while (index <= prevVal.length) {
					index++;
				}
				sliced = qtyCode.value.slice(index - 1);
			}
		}
		if (sliced.length != 0) {
			qtyCode.value = sliced;
		}
		SetField(qtyCode.value, '#qtyForm input','#qtyForm select','add');
		let prevNum = inpQuant1.value;
		if (inpQuant1.value == "" && prevNum == "" || qtyCode.value !== prevVal) {
			inpQuant1.value = 1;
		}
		else if (qtyCode.value === prevVal && prevVal != "" && sliced != "") {
			inpQuant1.value = Number(prevNum) + 1;
		}
		prevVal = sliced;
	}
}
//#endregion

//#region set fields on form
function SetField(codeToProcess, input, select,type) {
	filterArray(codeToProcess.toLowerCase());
	var input = document.querySelectorAll(input);
	var select = document.querySelectorAll(select);

	input.forEach(input => {
		if (input.tagName.toLowerCase() === 'input') {
			// Do something with it, for example:
			filtered.forEach(item => {
				var qty = extractNum(item.in_quantity);
				var sz = extractNum(item?.in_size);
				if (input.id == 'in_code2') {
					input.value = item.in_code;
				}
				if (input.id == 'in_name1' || input.id == 'in_name2') {
					input.value = item.in_name;
				}
				if (input.id == 'inpQuant1' || input.id == 'inpQuant2') {
					if (type != 'add') {
						input.value = Number(qty.num);
					}
				}
				if (input.id == 'inpSize1' || input.id == 'inpSize2') {
					if (sz.num != 0) {
						input.value = Number(sz.num);
					}
				}
				if (input.id == 'inpDate' || input.id == 'inpDate2') {
					input.value = item.in_dateAdded;
				}
			});
		}
	});
	select.forEach(input => {
		if (input.tagName.toLowerCase() === 'select') {
			if (input.id == 'selcat1' || input.id == 'selcat2') {
				for (var index = 0; index < input.options.length; index++) {
					let option = input.options[index].value;
					if (filtered[0].in_category === option) {
						input.selectedIndex = index;
					}
				}
			}
			if (input.id == 'selclass' || input.id == 'selclass2') {
				for (var index = 0; index < input.options.length; index++) {
					let option = input.options[index].value;
					if (filtered[0].in_class === option) {
						input.selectedIndex = index;
					}
				}
			}
			if (input.id == 'seltype' || input.id == 'seltype2') {
				for (var index = 0; index < input.options.length; index++) {
					let option = input.options[index].value;
					if (filtered[0].in_type === option) {
						input.selectedIndex = index;
					}
				}
			}
			if (input.id == 'inpQuant' || input.id == 'inpQuant2A') {
				var quant = extractNum(filtered[0].in_quantity);
				for (var index = 0; index < input.options.length; index++) {
					let option = input.options[index].value;
					if (quant.measurement === option) {
						input.selectedIndex = index;
					}
				}
			}
			if (input.id == 'inpSize' || input.id == 'inpSize2A') {
				var sze = extractNum(filtered[0].in_size);
				var newUnit = "";
				if (sze.measurement == ".IN" || sze.measurement == "X.IN" || sze.measurement == "XIN") {
					newUnit = 'IN';
				} else if (sze.measurement == ".IN,LENGTH") {
					newUnit = 'LENGTH';
				}
				else {
					newUnit = sze.measurement;
				}
				for (var index = 0; index < input.options.length; index++) {
					let option = input.options[index].value;
					if (newUnit === option) {
						input.selectedIndex = index;
					}
				}
			}
		} 
	});
	//GetBarcode()
}
//#endregion

function OpenEdit(invCode) {
	editOpenPopupInv();
	var inputs = document.querySelector('#editForm').elements;
	AppendOption(editFormSel);
	SetField(invCode, '#editForm input', '#editForm select', 'edit');
}