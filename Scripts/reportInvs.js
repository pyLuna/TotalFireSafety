﻿let jsonArray = [];
let filtered = [];
let itemCategories = [];
let newArray = [];
let fixedArray = [];
var array = [];
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
var searchTerm = '';
var selectedCategory = '';
var startDate = null;
var endDate = null;



document.querySelector('#startDate').addEventListener('change', function () {
    startDate = new Date(this.value);
    console.log(startDate.toLocaleDateString('en-US'));
    filterTable();
});

document.querySelector('#endDate').addEventListener('change', function () {
    endDate = new Date(this.value);
    console.log(endDate.toLocaleDateString('en-US'));
    filterTable();
});

// Add event listeners to the search input, category select element, and date inputs
document.querySelector('.search-wrapper input').addEventListener('keyup', function () {
    searchTerm = this.value;
    filterTable();
});

document.querySelector('#selcat').addEventListener('change', function () {
    selectedCategory = this.value;
    filterTable();
});



function setTable(array) {
    table.innerHTML = '';
    var size = '';
    var filteredArray = array;

    // Filter the array based on the search term, selected category, and date range
    if (searchTerm !== '') {
        filteredArray = filteredArray.filter(function (item) {
            return item.Inventory.in_name.toLowerCase().includes(searchTerm.toLowerCase());
        });
    }

    if (selectedCategory !== '') {
        filteredArray = filteredArray.filter(function (item) {
            return item.Inventory.in_category === selectedCategory;
        });
    }

    if (startDate && endDate) {
        filteredArray = filteredArray.filter(function (item) {
            const updateDate = new Date(Date.parse(item.update_date));
            return updateDate >= startDate && updateDate <= endDate;
        });
    }

    if (sortByColumnName) {
        filteredArray.sort(function (a, b) {
            const aLabel = a[sortByColumnName].toLowerCase()[0];
            const bLabel = b[sortByColumnName].toLowerCase()[0];
            if (aLabel < bLabel) return -1;
            if (aLabel > bLabel) return 1;
            return 0;
        });
    }

    // Display the filtered data in the table
    if (filteredArray.length !== 0) {
        for (var i = 0; i < filteredArray.length; i++) {
            const matchResult = array[i].update_quantity.match(/\d+/);
            const updateQuantity = matchResult ? parseInt(matchResult[0]) : 0;


            const timestamp = array[i].update_date.substring(6, 19);
            const date = new Date(parseInt(timestamp));
            const month = ("0" + (date.getMonth() + 1)).slice(-2);
            const day = ("0" + date.getDate()).slice(-2);
            const year = date.getFullYear();
            const formattedDate = `${date.getMonth() + 1}/${date.getDate()}/${date.getFullYear()}`;


            var row = `<tr>`;
            row += `<td><label>${formattedDate}</label></td>`;
            row += `<td id='update_item_id'><label>${array[i].update_item_id}</label></td>`;
            row += `<td id="inventory.in_name"><label>${array[i].Inventory.in_name}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_category}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_type}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_size === null ? "" : array[i].Inventory.in_size}</label></td>`;
            row += `<td><label>${array[i].update_quantity}</label></td>`;

            if (updateQuantity <= 50) {
                row += `<td><label style="color: red">critical</label></td>`;
            } else if (updateQuantity >= 51 && updateQuantity <= 100) {
                row += `<td><label style="color: yellow">average</label></td>`;
            } else if (updateQuantity >= 101) {
                row += `<td><label style="color: green">Standard</label></td>`;
            } else {
                row += `<td><label></label></td>`;
            }

            row += `<td name="inventory.in_class"><label>${array[i].Inventory.in_class}</label></td>`;
            row += `</tr>`;

            // Get the label element within the row and set its class attribute

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


//#region Search Item
function filterTable() {
    var tableRows = document.querySelectorAll('#myTable tbody tr');

    tableRows.forEach(function (row) {
        var itemName = row.querySelector('td:nth-child(3) label').textContent.toLowerCase();
        var itemCategory = row.querySelector('td:nth-child(4) label').textContent;
        var updateDate = row.querySelector('td:nth-child(1) label').textContent;
        var date = new Date(Date.parse(updateDate));

        if ((itemName.includes(searchTerm.toLowerCase()) || searchTerm === '') &&
            (itemCategory === selectedCategory || selectedCategory === '') &&
            (startDate === null || startDate === 'mm/dd/yyyy' || date >= startDate) &&
            (endDate === null || endDate === 'mm/dd/yyyy' || date <= endDate)) {
            row.style.display = 'table-row';
        } else {
            row.style.display = 'none';
        }
    });
}



function sortTableByColumnName(columnName) {
    var rows = Array.from(table.querySelectorAll('tr'));

    // Determine the index of the column to sort by
    var index;
    switch (columnName) {
        case 'update_item_id':
            index = 1;
            break;
        case 'inventory.in_name':
            index = 2;
            break;
        case 'inventory.in_class':
            index = 8;
            break;
        default:
            return;
    }

    // Sort the rows based on the contents of the specified column
    rows.sort(function (rowA, rowB) {
        var cellA = rowA.querySelectorAll('td')[index].textContent.trim();
        var cellB = rowB.querySelectorAll('td')[index].textContent.trim();

        if (columnName === 'inventory.in_class') {
            // Sort by the first letter of the class name
            return cellA.charCodeAt(0) - cellB.charCodeAt(0);
        } else {
            // Sort alphabetically by default
            return cellA.localeCompare(cellB);
        }
    });

    // Remove the current rows from the table
    rows.forEach(function (row) {
        row.parentNode.removeChild(row);
    });

    // Add the sorted rows back to the table
    rows.forEach(function (row) {
        table.appendChild(row);
    });
}