let jsonArray = [];
let filtered = [];
let itemCategories = [];
let newArray = [];
let fixedArray = [];
let table = document.querySelector('#myTable tbody');
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');
let category = document.getElementById('category');
let class1 = document.getElementById('class');
let type = document.getElementById('type');
let itemCode = document.getElementById('itemCode');
let allSelect = document.querySelectorAll('#myForm select');

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
			row += `<td id="hideActionBtn"><div class="inventory-action-style">`;
			row += `<button class="qty-add-btn" title="ADD QUANTIYY SELECTED ITEM" onclick="canOpenPopup()"> <a href="#"><span class="las la-plus"></span></a></button>`;
			row += `<button class="edit-btn" title="EDIT SELECTED ITEM" onclick="addOpenPopupInv()"> <a href="#"><span class="lar la-edit"></span></a></button>`;
			row += `<button class="del-btn" title="DELETE SELECTED ITEM" onclick = "canOpenPopup('${array[i].in_code}')"> <a href="#"><span class="lar la-trash-alt"></span></a></button>`;
			row += `</div></td>`;
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

	//let array = [];
	//let catvalue = selcat.options[selcat.selectedIndex].value.toLocaleLowerCase();
	//let filvalue = filter.options[filter.selectedIndex].value.toLocaleLowerCase();
	//if (filter.selectedIndex != 0 && selcat.selectedIndex != 0) {
	//    array = filtered;
	//}
	//else {
	//    array = fixedArray;
	//}
	//for (let j = 0; j < array.length; j++) {
	//    if (filter.selectedIndex != 0 && selcat.selectedIndex == 0 || selcat.selectedIndex != 0 && filter.selectedIndex == 0) {
	//        if (JSON.stringify(array[j].in_category).toLowerCase().includes(value) || JSON.stringify(array[j].in_remarks).toLowerCase().includes(value)) {
	//            filtered.push(array[j]);
	//        }
	//    }
	//    else {
	//        if (JSON.stringify(array[j].in_category).toLowerCase().includes(catvalue) && JSON.stringify(array[j].in_remarks).toLowerCase().includes(filvalue)) {
	//            newArray.push(array[j]);
	//        }
	//    }
	//}
	//newArray.length = 0;
}
//#endregion

//#region set the attributes of the new item form

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

function AppendOption() {
	var optionValue = '';
	allSelect.forEach((item) => {

		newArray.length = 0;
		var id = item.getAttribute('id');
		var sel = document.getElementById(id) === null ? 0 : document.getElementById(id);

		fixedArray.forEach(function (item) {
			if (id == 'category') {
				optionValue = 'Select Category';
				newArray.push(item.in_category);
			}
			if (id == 'class') {
				optionValue = 'Select Class';
				newArray.push(item.in_class);
			}
			if (id == 'type') {
				optionValue = 'Select Type';
				newArray.push(item.in_type);
			}
			if (id == 'sizeSel') {
				optionValue = 'Unit';
				let sizeResult = extractNum(item.in_size);
				newArray.push(sizeResult.measurement);
			}
			if (id == 'sizeQuant') {
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

	
}

//#endregion

function SetItemCode() {
	var catVal = category.options[category.selectedIndex].value;
	var classVal = class1.options[class1.selectedIndex].value;
	var code = itemCode.options[itemCode.selectedIndex].value;

	var codeabbr = "";
	var categoryabbr = "";
	var nameabbr = "";


	var string = `${codeabbr}-${categoryabbr}-${nameabbr}`;


}
function abbreviateSentence(sentence) {
	if (!sentence) {
		return null;
	}

	var words = sentence.split(' ');
	var abbreviation = '';

	words.forEach(function (word) {
		var randomIndex = Math.floor(Math.random() * word.length);
		abbreviation += word.charAt(randomIndex).toUpperCase();
	});

	return abbreviation;
}