let jsonArray = [];
let filtered = [];
let fixedArray = [];
const table = document.querySelector('#myTable tbody');

function GetAll() {

    fetch('/Admin/FindDataOf?requestType=deleted')
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

            if (array[i].in_category == null) {
                array[i].in_category = '';
            }
            if (array[i].in_type == null) {
                array[i].in_type = '';
            }
            if (array[i].in_size == null || array[i].in_size == "") {
                array[i].in_size = '';
            }
            var row = `<tr>`;
            row += `<td id="in_code"><label>${array[i].in_code}</label></td>`;
            row += `<td name="in_name"><label>${array[i].in_name}</label></td>`;
            row += `<td name="in_category"><label>${array[i].in_category}</label></td>`;
            row += `<td name="in_type"><label>${array[i].in_type}</label></td>`;
            row += `<td name="in_size"><label>${array[i].in_size}</label></td>`;
            row += `<td name="in_quantity"><label>${array[i].in_quantity}</label></td>`;
            row += `<td name="in_class"><label>${array[i].in_class}</label></td>`;
            row += `<td></td>`;
            row += `<td id="hideActionBtn"><div class="inventory-action-style">`;
            row += `<button class="edit-btn" title="RESTORE ITEM" onclick="RestoreItem('${array[i].in_code}')"> <a href="#"><span class="las la-trash-restore"></span></a></button>`;
            row += `</div></td>`;
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
        errorMessageRow.innerHTML = "<td colspan='9' style='text-align:center;'>Item Not found<td>";
        //console.log(res.statusText);
        table.appendChild(errorMessageRow);
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

function filterArray(value) {
    filtered.length = 0;
    for (var j = 0; j < fixedArray.length; j++) {
        if (JSON.stringify(fixedArray[j]).toLowerCase().includes(value)) {
            filtered.push(fixedArray[j]);
        }
    }
}

function SortByCategory(value) {
    const category = document.querySelector('#selcat');
    //document.getElementById("myDropdown").setAttribute("style","display:none;");
    if (value != 'Clear') {
        //category.value = value;
        SearchItem(value.toLowerCase());
    }
    else {
        category.selectedIndex = 0;
        setTable(fixedArray);
    }
}

function RestoreItem(value) {
    fetch('/Admin/RestoreItem?itemCode=' + value, {
        method: "POST"
    })
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                //lagay error dito
            }
        })
        .then(data => {
            console.log(data);
        })
        .catch(error => {
            console.error(error);
        });
}