let jsonArray = [];
let filtered = [];
let fixedArray = [];
const table = document.querySelector('#myTable tbody');

function GetAll() {

    fetch('/Admin/FindDataOf')
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
    if (array.length != 0) {
        for (var i = 0; i < array.length; i++) {

            if (array[i].in_category === null) {
                array[i].in_category = '';
            }
            if (array[i].in_type === null) {
                array[i].in_type = '';
            }
            if (array[i].in_size === null) {
                array[i].in_size = '';
            }
            var row = `<tr>`;
            row += `<form action="/Admin/Inventory?item=${array[i].in_code}" method="post" id="tableList">`;
            row += `<td id="in_code"><label>${array[i].in_code}</label></td><td name="in_name"><label>${array[i].in_name}</label></td><td name="in_category"><label>${array[i].in_category}</label></td><td name="in_type"><label>${array[i].in_type}</label></td><td name="in_size"><label>${array[i].in_size}</label></td><td name="in_quantity"><label>${array[i].in_quantity}</label></td>`;
            row += `</form>`;
            row += `<td id="hideActionBtn"><div class="inventory-action-style">`;
            row += `<button class="del-btn" title="DELETE SELECTED ITEM" onclick="canOpenPopup()"> <a href="#"><span class="lar la-trash-alt"></span></a></button>`;
            row += `</td></div>`;
            row += `</tr>`;
            table.innerHTML += row;
            filtered.length = 0;
        }

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

function filterArray(value) {
    filtered.length = 0;
    for (var j = 0; j < fixedArray.length; j++) {
        if (JSON.stringify(fixedArray[j]).toLowerCase().includes(value)) {
            filtered.push(fixedArray[j]);
        }
    }
}

function SearchItem(value) {
    filterArray(value.toLowerCase());
    if (value == '') {
        setTable(fixedArray);
    } else {
        setTable(filtered);
    }
}

function SortByCategory(value) {
    const category = document.querySelector('#selcat');
    //document.getElementById("myDropdown").setAttribute("style","display:none;");
    if (value != '') {
        category.innerHTML = value;
        SearchItem(value.toLowerCase());
    }
    else {
        category.innerHTML = 'Select Category';
        setTable(fixedArray);
    }
}

function setField(value) {
    let name = document.querySelector('#itemName');
    let size = document.querySelector('#itemSize');
    let quantity = document.querySelector('#itemQuant');
    let cat = document.getElementById("cat-select");
    let type = document.getElementById("type-select");

    for (var i = 0; i < fixedArray.length; i++) {
        if (fixedArray[i].in_code == value) {
            filtered.length = 0;
            filtered.push(fixedArray[i]);
        }
    }
    name.value = filtered[0].in_name;
    size.value = filtered[0].in_size;
    quantity.value = filtered[0].in_quantity;

    // loop through all the options in the select element
    for (let i = 0; i < cat.options.length; i++) {
        let option = cat.options[i];
        // check if the option value exists in the valuesToCompare array
        if (filtered[0].in_category === option.value) {
            cat.selectedIndex = i;
        }
    }

    for (let i = 0; i < type.options.length; i++) {
        let option = type.options[i];
        // check if the option value exists in the valuesToCompare array
        if (filtered[0].in_type === option.value) {
            type.selectedIndex = i;
        } 
    }
   
}

function exportArrayToCsv() {
    let newArray = [ ['Code','Name','Category','Type','Size','Quantity']];

    for (var i = 0; i < fixedArray.length; i++) {
        newArray.push([fixedArray[i].in_code, fixedArray[i].in_name, fixedArray[i].in_category, fixedArray[i].in_type, fixedArray[i].in_size, fixedArray[i].in_quantity]);
    }
    var csv = newArray.map(row => row.join(',')).join('\n');
    // Create a hidden link
    var link = document.createElement('a');
    link.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv));
    link.setAttribute('download', 'data.csv');

    //// Trigger the download
    link.click();
}