let jsonArray = [];
let filtered = [];
let fixedArray = [];
let Employees = [];
let newEmployee = [];
let itemInv = [];
let typeLabel = ['Deploy', 'Purchase', 'Supply'];
const table = document.querySelector('#myTable tbody');
let position = document.getElementById('position');
let employeeId = document.getElementById('employeeId');
let reqtype = document.getElementById('reqtype');
let reqdate = document.getElementById('reqdate');
let reqid = document.getElementById('reqid');
let EmployeeInput = document.getElementById("EmployeeInput");
let itemList = document.getElementById("itemList");
const formTable = document.querySelector('#formTable tbody');

function GetAllItem() {
    fetch('/Admin/FindDataOf?requestType=inventory')
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log("error fetch");
            }
        })
        .then(data => {
            jsonArray.length = 0;
            jsonArray.push(data);
            fixArray(jsonArray, 3);
        })
        .catch(error => {
            console.error(error);
        });
}
function GetAllEmployee() {
    fetch('/Admin/SearchEmployee')
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log("error fetch");
            }
        })
        .then(data => {
            Employees.push(data);
            fixArray(Employees, 2);
        })
        .catch(error => {
            console.error(error);
        });
}
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
            jsonArray.length = 0;
            jsonArray.push(data);
            fixArray(jsonArray, 1);
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
    if (boolean == 1) {
        for (var j = 0; j < array[0].length; j++) {
            fixedArray.push(array[0][j]);
        }
    }
    else if (boolean == 2) {
        for (var j = 0; j < array[0].length; j++) {
            newEmployee.push(array[0][j]);
        }
    }
    else {
        for (var j = 0; j < array[0].length; j++) {
            itemInv.push(array[0][j]);
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
    newEmployee.forEach(function (item) {
        var option = document.createElement('option');
        option.value = item.emp_name;
        itemList.appendChild(option);
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

function createOption() {
    let select = document.createElement('select');
    let option = "";

    for (var i = 0; i < itemInv.length; i++) {
        //option.value = typeLabel[i];

        option += `<option value="${itemInv[i]}">${itemInv[i]}</option>`;

        select.innerHTML = option;
    }
    return select;
}

function AddForm(value) {
    newEmployee.forEach(function (item) {
        if (item.emp_name == value) {
            var datenow = getDateNow();
            employeeId.innerHTML = ` ${item.emp_no}`
            position.innerHTML = ` ${item.emp_position}`
            reqdate.innerHTML = ` ${datenow}`
        }
    });
    createRow();
}

function setReqId(item) {
    let value = EmployeeInput.value;
    let lastIndex = fixedArray.length - 1;
    let str = "";
    if (item === 1) {
        str = "DEP";
    }
    if (item === 2) {
        str = "PUR";
    }
    if (item === 3) {
        str = "SUP";
    }
    console.log(fixedArray[lastIndex].request_type_id);
    let newid = Number(fixedArray[lastIndex].request_type_id) + 1;
    reqid.innerHTML = ' ' +str + newid;
}

function createRow() {
    var td = document.createElement('td');
    var data = "";
    
    data += `<td><label></label></td>`;
    data += `<td contenteditable="true"><label>category</label></td>`;
    data += `<td contenteditable="true"><label>size</label></td>`;
    data += `<td contenteditable="true"><label>quantity</label></td>`;
    data += `<td contenteditable="true"><label>type</label></td>`;
    data += `<td contenteditable="true"><label>class</label></td>`;
    data += `</tr>`;

    formTable.innerHTML += data;
}