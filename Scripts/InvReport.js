let jsonArray = [];
let fixedArray = [];
let filteredArray = [];
const fromDateInput = document.querySelector('#dt1');
const toDateInput = document.querySelector('#dt2');
const table = document.querySelector('#myTable tbody');
let selcat = document.getElementById('selcat');
let filter = document.getElementById('filter');


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


            const timestamp = array[i].update_date.substring(6, 19);
            const date = new Date(parseInt(timestamp));
            const month = ("0" + (date.getMonth() + 1)).slice(-2);
            const day = ("0" + date.getDate()).slice(-2);
            const year = date.getFullYear();
            const formattedDate = `${month}/${day}/${year}`;


            var row = `<tr>`;
            row += `<td><label>${formattedDate}</label></td>`;
            row += `<td id='update_item_id'><label>${array[i].update_item_id}</label></td>`;
            row += `<td id='inventory.in_name'><label>${array[i].Inventory.in_name}</label></td>`;
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

            row += `<td name='inventory.in_class'><label>${array[i].Inventory.in_class}</label></td>`;
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