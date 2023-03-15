let jsonArray = [];
let fixedArray = [];
let filteredArray = [];
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

            var row = `<tr>`;
            row += `<td><label>${array[i].update_item_id}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_name}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_category}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_type}</label></td>`;
            row += `<td><label>${array[i].Inventory.in_size === null ? "" : array[i].Inventory.in_size}</label></td>`;
            row += `<td><label>${array[i].update_quantity}</label></td>`;
            row += `<td><label>${array[i].update_type}</label></td>`;
            row += `<td><label>${array[i].FormattedDate}</label></td>`;
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