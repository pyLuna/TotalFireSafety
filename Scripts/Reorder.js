let jsonArray = [];
let filtered = [];
let itemCategories = [];
let newArray = [];
let fixedArray = [];
var array = [];
let table = document.querySelector('#myTable tbody');


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
function Sort(item, value) {
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
        catcher = SortAscending(arrayToSend, value);
    } else {
        iElement.classList.remove('la-sort-up');
        iElement.classList.add('la-sort-down');
        catcher = SortDescending(arrayToSend, value);
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
function FilterItem(value, array) {
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
//#endregion}