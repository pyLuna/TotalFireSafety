let tableData = [];
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
let select_type = document.getElementById("select_type");
let table1 = document.getElementById("formTable");

const formTable = document.querySelector('#formTable tbody');

formTable.addEventListener("click", function (event) {
    const target = event.target;

    if (target.tagName.toLowerCase() === "a" && target.classList.contains("delete-row")) {
        const row = target.parentNode.parentNode;
        row.parentNode.removeChild(row);
    }
});

document.addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        createRow();
    }
});

const template = {
    "request_id": "",
    "request_type": "", 
    "request_item": "",
    "request_item_quantity": "",
    "request_date": "",
    "request_employee_id": "",
    "request_status": "",
    "request_type_id": ""
};

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
            console.log(`inventory`);
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
            console.log(`Employee`);
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
    //let select = document.createElement('select');
    //let option = "";

    //itemInv.forEach(function (item) {
    //    option += `<option value="${item.in_name}">${item.in_name}</option>`;
    //    select.innerHTML = option;
    //})
    //select.style.border = "none";
    //return select.outerHTML;
    let datalist = document.createElement('datalist');
    let option = "";
    itemInv.forEach(function (item) {
        option += `<option value="${item.in_name}">${item.in_name}</option>`;
        datalist.innerHTML = option;
    });
    datalist.setAttribute('id', 'itemoption');
    return datalist.outerHTML;
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
    var select = createOption();
    var data = "";
    var increment = localStorage.getItem('increment');
    var newNum = 0;

    if (increment == null) {
        localStorage.setItem('increment', 1);
    } else {
        newNum = Number(increment) + 1;
        localStorage.setItem('increment', newNum);
    }
    //oninput = "setRowData(this.value,this.id)"
    data += `<tr>`;
    data += `<td><input id="itemInp${newNum}" type="text" list="itemoption" oninput="getInputId(this.id)" style="width:150px;height:30px;border:none;">${select}</td>`;
    data += `<td contenteditable="false" id="itemCat${newNum}"></td>`;
    data += `<td contenteditable="true" id="itemSize${newNum}"></td>`;
    data += `<td contenteditable="true" id="itemQuant${newNum}"></td>`;
    data += `<td contenteditable="false" id="itemType${newNum}"></td>`;
    data += `<td contenteditable="false" id="itemClass${newNum}"></td>`;
    data += `<td><a style="text-decoration:underline;" href="#" id="delBtn${newNum}" class="delete-row">Delete</a></td>`;
    //data += `<td><button>Delete</button></td>`;
    data += `</tr>`;

    formTable.innerHTML += data;
}

function extractNum(value) {
    let num = 0;
    let measurement = '';
    for (let i = 0; i < value?.length; i++) {
        if (!isNaN(parseInt(value[i]))) {
            num = num * 10 + parseInt(value[i]);
            }
    }
    return num;
}

function getInputId(id) {
    let inputId = document.getElementById(id);
    let increment = extractNum(id);

    setRowData(inputId.value, increment);
}

function setRowData(value, id) {

    let itemCat = document.getElementById(`itemCat${id}`);
    let itemType = document.getElementById(`itemType${id}`);
    let itemQuant = document.getElementById(`itemQuant${id}`);
    let itemClass = document.getElementById(`itemClass${id}`);
    let itemSize = document.getElementById(`itemSize${id}`);
    itemInv.forEach(function (item) {
        if (item.in_name == value) {
            itemCat.innerHTML = `${item.in_category}`;
            itemClass.innerHTML = `${item.in_class}`;
            itemType.innerHTML = `${item.in_type}`;

            if (item.in_type == null || item.in_type == "") {
                itemType.innerHTML = '';
            }

        }
    });
}

function deleteAllRows() {
    while (table1.rows.length > 1) { // Keep header row
        table1.deleteRow(1); // Delete row at index 1 (second row)
    }
}

function resetForm() {
    EmployeeInput.value = "";
    select_type.selectedIndex = 0;
    employeeId.innerHTML = "";
    position.innerHTML = "";
    reqdate.innerHTML = "";
    reqid.innerHTML = "";
    deleteAllRows();
}

function saveRequest() {

    for (var row = 1; row < table1.rows.length; row++) {
        let rowData = [];
        for (var cell = 0; cell < table1.rows[row].cells.length - 1; cell++) {
            var newValue = "";

            if (cell == 0) {
                //newValue = 
            }

            rowData.push(table1.rows[row].cells[cell].textContent);
        }
        tableData.push(rowData);
    }
    console.log(tableData);
}

function setTemplateData() {

}