let jsonArray = [];
let filtered = [];
let itemCategories = [];
let newArray = [];
let fixedArray = [];
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');
const fromDateInput = document.querySelector('#dt1');
const toDateInput = document.querySelector('#dt2');
const table = document.querySelector('#myTable tbody');




function GetAll() {
    fetch('/Admin/FindDataOf?requestType=report')
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
                errorMessageRow.innerHTML = '<td colspan="8">Loading Error<td>';
                table.appendChild(errorMessageRow);
            }
        })
        .then(data => {
            jsonArray.length = 0;
            jsonArray.push(data);
            fixArray(jsonArray);
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
    for (var j = 0; j < jsonArray[0].length; j++) {
        fixedArray.push(jsonArray[0][j]);
    }
}

function setTable(array) {
    table.innerHTML = '';
    var size = '';
    if (array.length != 0) {
        for (var i = 0; i < array.length; i++) {
            const matchResult = array[i].update_quantity.match(/\d+/);
            const updateQuantity = matchResult ? parseInt(matchResult[0]) : 0;
           
            let remarks = '';
            let remclass = "";

            if (updateQuantity <= 50) {
                remarks = 'critical';
                remclass = '#FF0000';
                
            } else if (updateQuantity >= 51 && updateQuantity <= 100 )  {
                remarks = 'average';
                remclass = '#FFFF00';
            } else if (updateQuantity >= 101) {
                remarks = 'Standard';
                remclass = '#7CFC00';
               
            } else {
                remarks = ''; // set to empty string if no update_quantity value
            }
            

            const timestamp = array[i].update_date.substring(6, 19);
            const date = new Date(parseInt(timestamp));
            const month = date.getMonth() + 1;
            const day = date.getDate();
            const year = date.getFullYear();
            const formattedDate = `${month}/${day}/${year}`;


            var row = `<tr>`;
            row += `<td><label>${formattedDate}</label></td>`;
            row += `<td id='update_item_id'><label>${array[i].update_item_id}</label></td>`;
            row += `<td name="inventory.in_name"><label>${array[i].Inventory.in_name}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_category}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_type}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_size === null ? "" : array[i].Inventory.in_size}</label></td>`;
            row += `<td><label>${array[i].update_quantity}</label></td>`;
            row += `<td><label class="${remclass}">${remarks}</label></td>`;
            row += `<td name="inventory.in_class"><label>${array[i].Inventory.in_class}</label></td>`;
            row += `</tr>`;
            table.innerHTML += row;
        }
        filtered.length = 0;
    }
    else {
        //error handler if input value not found
        table.innerHTML = " ";
        const errorMessageRow = document.createElement('tr');
        errorMessageRow.style.textAlign = "center";
        errorMessageRow.style.fontStyle = "italic";
        errorMessageRow.innerHTML = "<td colspan='6'>Item Not found<td>";
        //console.log(res.statusText);
        table.appendChild(errorMessageRow);
    }
}
function filterByDate() {
    const startDate = new Date(document.getElementById("dt1").value);
    const endDate = new Date(document.getElementById("dt2").value);
    const filteredArray = fixedArray.filter((item) => {
        const itemDate = new Date(item.FormattedDate);
        return itemDate >= startDate && itemDate <= endDate;
    });
    setTable(filteredArray);
}

fromDateInput.addEventListener('change', filterByDateRange);
toDateInput.addEventListener('change', filterByDateRange);
//Call

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
            if (JSON.stringify(array[j].Inventory.in_category).toLowerCase().includes(catvalue)) {
                filtered.push(array[j]);
            }
        }
        else {
            if (JSON.stringify(array[j].Inventory.in_category).toLowerCase().includes(value)) {
                filtered.push(array[j]);
            }
        }
    }
}
//#endregion
