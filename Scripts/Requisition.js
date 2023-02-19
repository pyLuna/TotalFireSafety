let jsonArray = [];
let filtered = [];
let fixedArray = [];
let labelName = ['Position', 'Employee ID', 'Request Type', 'Request Date', 'Request ID'];
const table = document.querySelector('#myTable tbody');
let position = document.getElementById('position');
let employeeId = document.getElementById('employeeId');
let reqtype = document.getElementById('reqtype');
let reqdate = document.getElementById('reqdate');
let reqid = document.getElementById('reqid');
let EmployeeInput = document.getElementById("itemList");

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
            jsonArray.push(data);
            fixArray(jsonArray, true);
            if (table !== null) {
                setTable(fixedArray);
            }
            console.log(fixedArray);
        })
        .catch(error => {
            console.error(error);
        });
}
function fixArray(array, boolean) {
    if (boolean) {
        for (var j = 0; j < array[0].length; j++) {
            fixedArray.push(array[0][j]);
        }
    }
    else {
        for (var j = 0; j < array[0].length; j++) {
            newEmployee.push(array[0][j]);
        }
    }
}

function setTable(array) {
    table.innerHTML = '';
    var className = "";
    if (array.length != 0) {
        for (var i = 0; i < array.length; i++) {

            if (array[i].request_status === "pending") {
                className = "stat-pen";
            }
            if (array[i].request_status === "decline") {
                className = "stat-dec";
            }
            if (array[i].request_status === "approved") {
                className = "stat-appr";
            }

            var row = `<tr>`; /*onclick = "canOpenPopup()"*/
            row += `<td><label>${array[i].Id}</label></td>`;
            row += `<td><label>${array[i].request_type}</label></td>`;
            row += `<td><label>${array[i].Employee.emp_name}</label></td>`;
            row += `<td><label>${array[i].Employee.emp_no}</label></td>`;
            row += `<td><label>${array[i].FormattedDate}</label></td>`;
            row += `<td><label class="${className}" style="font-weight:bold;">${array[i].request_status}</label></td>`;
            //row += `<td id="in_code"><label>${array[i].in_code}</label></td><td name="in_name"><label>${array[i].in_name}</label></td><td name="in_category"><label>${array[i].in_category}</label></td><td name="in_type"><label>${array[i].in_type}</label></td><td name="in_size"><label>${array[i].in_size}</label></td><td name="in_quantity"><label>${array[i].in_quantity}</label></td><td name="in_class"><label>${array[i].in_class}</label></td>`;
            row += `<td id="hideActionBtn">`;
            row += `<div class="purchase-action-style">`;
            row += `<button class="acc-btn" title="ACCEPT REQUEST" onclick="infoOpenPopup()"> <a href="#"><span class="las la-check-circle"></span></a></button>`;
            row += `<button class="dec-btn" title="DECLINE REQUEST" onclick="canOpenPopup()"> <a href="#"><span class="las la-times-circle"></span></a></button>`;
            row += `<button class="pri-btn" title="PRINT REPORT" > <a href="#"><span class="las la-print"></span></a></button>`;
            row += `<button class="expo-btn" title="EXPORT REPORT" > <a href="#"><span class="las la-file-download"></span></a></button>`;
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
        errorMessageRow.innerHTML = "<td colspan='6'>Item Not found<td>";
        //console.log(res.statusText);
        table.appendChild(errorMessageRow);
    }
}

function EmployeeList() {
    fixedArray.forEach(function (item) {
        var option = document.createElement('option');
        option.value = item.Employee.emp_name;
        EmployeeInput.appendChild(option);
    });
}

function setForm(value,formType) {
    if (formType == "add") {
        AddForm(value);
    }
}

function getDateNow() {
    const now = new Date();
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    const formattedDate = now.toLocaleDateString('en-US', options);
    return formattedDate;
}

function AddForm(value) {
    fixedArray.forEach(function (item) {
        if (item.Employee.emp_name == value) {
            var datenow = getDateNow();
            employeeId.innerHTML = `<a style="font-weight:bold;">Employee ID:</a> ${item.Employee.emp_no}`
            position.innerHTML = `<a style="font-weight:bold;">Position:</a> ${item.Employee.emp_position}`
            reqtype.innerHTML = `<a style="font-weight:bold;">Request Type:</a> ${item.request_type}`
            reqdate.innerHTML = `<a style="font-weight:bold;">Request Date:</a> ${datenow}`
            reqid.innerHTML = `<a style="font-weight:bold;">Request ID:</a> ${item.Id}`
        }
    });
}