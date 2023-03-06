let inputVal = [];
let viewData = [];
let codeList = [];
let tableData = [];
let jsonArray = [];
let filtered = [];
let allRequests = [];
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
let requestType = "";
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
    "request_id": null,
    "request_type": "", 
    "request_item": "",
    "request_item_quantity": "",
    "request_date": "",
    "request_employee_id": "",
    "request_status": "",
    "request_type_id": 0
};

function GetAllItem() {
    fetch('/Admin/FindDataOf1?requestType=inventory')
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
            //window.location.replace('/Error/InternalServerError');
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
                PopulateView();
                setTable(viewData);
            }
        })
        .catch(error => {
            console.error(error);
        });
}
//serves as a search function
function filterArray(value) {
    filtered.length = 0;
    for (var j = 0; j < allRequests.length; j++) {
        if (JSON.stringify(allRequests[j]).toLowerCase().includes(value)) {
            filtered.push(allRequests[j]);
        }
    }
}

function SearchItem(value) {
    filterArray(value.toLowerCase());
    if (value == '') {
        setTable(allRequests);
    } else {
        setTable(filtered);
    }
}

function fixArray(array, boolean) {
    if (boolean == 1) {
        for (var j = 0; j < array[0].length; j++) {
            allRequests.push(array[0][j]);
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

            if (array[i].request_status === "Pending") {
                className = "stat-pen";
            }
            if (array[i].request_status === "Declined") {
                className = "stat-dec";
            }
            if (array[i].request_status === "Approved") {
                className = "stat-appr";
            }

            var row = `<tr>`;
            row += `<td><label>${array[i].Id}</label></td>`;
            row += `<td><label>${array[i].request_type}</label></td>`;
            row += `<td><label>${array[i].Employee.emp_name}</label></td>`;
            row += `<td><label>${array[i].Employee.emp_no}</label></td>`;
            row += `<td><label>${array[i].FormattedDate}</label></td>`;
            row += `<td><label class="${className}" style="font-weight:bold;">${array[i].request_status}</label></td>`;
            row += `<td id="hideActionBtn">`;
            row += `<div class="purchase-action-style">`;
            row += `<button class="acc-btn" title="ACCEPT REQUEST" onclick="UpdateStatus('${array[i].request_type_id}','approve')"> <a href="#"><span class="las la-check-circle"></span></a></button>`;
            row += `<button class="dec-btn" title="DECLINE REQUEST" onclick="UpdateStatus('${array[i].request_type_id}','decline')"> <a href="#"><span class="las la-times-circle"></span></a></button>`;
            row += `<button class="edit-btn" title="EDIT SELECTED ITEM" onclick="OpenEdit('${array[i].request_type_id}')"> <a href="#"><span class="lar la-edit"></span></a></button>`;
            row += `<button class="pri-btn" title="PRINT REPORT" > <a href="#"><span class="las la-print"></span></a></button>`;
            row += `<button class="expo-btn"  onclick="window.location.href='/Admin/ExportRequest?id=${array[i].request_type_id}'" title="EXPORT REPORT" > <a href="#"><span class="las la-file-download"></span></a></button>`;            row += `</div></td>`;
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
    itemList.innerHTML = "";
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

function createOption(num) {
    let datalist = document.createElement('datalist');
    let option = "";
    itemInv.forEach(function (item) {
        option += `<option value="${item.in_name}">${item.in_name}</option>`;
        datalist.innerHTML = option;
    });
    datalist.setAttribute('id', `itemoption${num}`);
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
    let newid = MaxRequestId() + 1;
    reqid.innerHTML = ' ' +str + newid;
}

function createRow() {
    var data = "";
    var increment = localStorage.getItem('increment');
    GetSetRowInput("get");
    var newNum = 0;
    if (increment == "null") {
        localStorage.setItem('increment', 1);
        newNum = localStorage.getItem('increment');
    } else {
        newNum = Number(increment) + 1;
        localStorage.setItem('increment', newNum);
    }
    var select = createOption(newNum);
    data += `<tr>`;
    data += `<td><input type="text" id="itemInp${newNum}" list="itemoption${newNum}" oninput="getInputId(this.id)" style="width:150px;height:30px;border:none;">${select}</td>`;
    data += `<td contenteditable="false" id="itemCat${newNum}"></td>`;
    data += `<td contenteditable="false" id="itemSize${newNum}"></td>`;
    data += `<td contenteditable="true" id="itemQuant${newNum}"></td>`;
    data += `<td contenteditable="false" id="itemType${newNum}"></td>`;
    data += `<td contenteditable="false" id="itemClass${newNum}"></td>`;
    data += `<td><a style="text-decoration:underline;" href="#" id="delBtn${newNum}" class="delete-row">Delete</a></td>`;
    data += `<td contenteditable="false" style="display:none;" id="itemCode${newNum}"></td>`;
    data += `</tr>`;
    formTable.innerHTML += data;
    GetSetRowInput("set");
}

function FindType(valueToValidate) {
    if (valueToValidate == "Deploy") {
        return 1;
    }
    if (valueToValidate == "Purchase") {
        return 2;
    }
    if (valueToValidate == "Supply") {
        return 3;
    }
}

function OpenEdit(idToFind) {
    filtered.length = 0;
    filterArray(idToFind);
    addOpenPopupPur();
    let index = 1;
    EmployeeInput.setAttribute('disabled', true);
    select_type.setAttribute('disabled', true);
    filtered.forEach(function (item) {
        createRow();
        let itemInp = document.getElementById(`itemInp${index}`);
        let itemCat = document.getElementById(`itemCat${index}`);
        let itemSize = document.getElementById(`itemSize${index}`);
        let itemQuant = document.getElementById(`itemQuant${index}`);
        let itemType = document.getElementById(`itemType${index}`);
        let itemClass = document.getElementById(`itemClass${index}`);
        EmployeeInput.value = item.Employee.emp_name;
        position.innerHTML = item.Employee.emp_position;
        select_type.selectedIndex = FindType(item.request_type);
        employeeId.innerHTML = item.Employee.emp_no;
        reqdate.innerHTML = item.FormattedDate;
        reqid.innerHTML = item.Id;
        itemInp.value = item.Inventory.in_name;
        itemCat.innerHTML = item.Inventory.in_category;
        itemSize.value = item.Inventory.in_size === null ? "" : item.Inventory.in_size;
        itemQuant.innerHTML = item.request_item_quantity;
        itemType.innerHTML = item.Inventory.in_type === null ? "" : item.Inventory.in_type;
        itemClass.innerHTML = item.Inventory.in_class;
        index++;
    });
    document.getElementById("formType").value = "edit";
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
    let itemCode = document.getElementById(`itemCode${id}`);
    itemInv.forEach(function (item) {
        if (item.in_name == value) {
            var newSize = item.in_size === null ? "" : item.in_size;
            var newType = item.in_type === null ? "" : item.in_type;
            itemCode.innerHTML = `${item.in_code}`;
            itemCat.innerHTML = `${item.in_category}`;
            itemClass.innerHTML = `${item.in_class}`;
            itemSize.innerHTML = `${newSize}`;
            itemType.innerHTML = `${newType}`;
        }
    });
}

function GetSetRowInput(method) {
    const rows = Array.from(table1.getElementsByTagName('tr')).slice(1);

    if (rows != 1) {
        if (method == "get") {
            inputVal.length = 0;
            for (var index = 0; index < rows.length; index++) {
                var value = GetInputValue(index);
                inputVal.push(value);
            }
        }
        else {
            for (var index = 0; index < rows.length - 1; index++) {
                const firstCell = rows[index].cells[0];
                const textbox = firstCell.querySelector('input[type="text"]');
                textbox.value = inputVal[index];
            }
        }
    }
    console.log(inputVal);
}

function deleteAllRows() {
    while (table1.rows.length > 1) { // Keep header row
        table1.deleteRow(1); // Delete row at index 1 (second row)
    }
}

function resetForm() {
    localStorage.setItem('increment', null);
    EmployeeInput.value = "";
    select_type.selectedIndex = 0;
    employeeId.innerHTML = "";
    position.innerHTML = "";
    reqdate.innerHTML = "";
    reqid.innerHTML = "";
    deleteAllRows();
}

function saveRequest() {
    var formType = document.getElementById("formType").value;
    tableData.length = 0;
    for (var row = 1; row < table1.rows.length; row++) {
        let rowData = [];
        for (var cell = 0; cell < table1.rows[row].cells.length - 1; cell++) {
            var newValue = "";
            let itemCode = document.getElementById(`itemCode${row}`)?.innerHTML;
            if (cell == 0) {
                newValue = GetInputValue(row-1);
            codeList.push(itemCode);
            }
            else {
                newValue = table1.rows[row].cells[cell].textContent
            }
            rowData.push(newValue);
        }
        tableData.push(rowData);
    }
    if (formType == "add") {
        setTemplate(tableData, formType);
    }
    else {
        setTemplate(filtered, formType);
    }
}

function setTemplate(array,formType) {
    let type = document.getElementById('select_type');
    let emp_no = document.getElementById('employeeId');
    let newRequest = [];

    if (formType == "add") {
        for (var i = 0; i < array.length; i++) {
            let newObj = Object.assign({}, template);
            newObj.request_date = getDateNow();
            newObj.request_employee_id = emp_no.innerHTML;
            newObj.request_type_id = MaxRequestId() + 1;
            newObj.request_type = type.options[type.selectedIndex].value;
            newObj.request_status = "pending";
            newObj.request_item = codeList[i];
            newObj.request_item_quantity = array[i][3];
            newRequest.push(newObj);
        }
    }
    //if (formType == "edit")
     else {
        const rows = Array.from(table1.getElementsByTagName('tr')).slice(1);
        for (var i = 0; i < array.length; i++) {
            let newObj = Object.assign({}, template);
            newObj.request_id = array[i].request_id;
            newObj.request_date = array[i].request_date;
            newObj.request_employee_id = array[i].request_employee_id;
            newObj.request_type_id = array[i].request_type_id;
            newObj.request_type = array[i].request_type;
            newObj.request_status = array[i].request_status;
            newObj.request_item = array[i].request_item;
            if (formType != "stats") {
                newObj.request_item_quantity = rows[i].cells[3].innerHTML;
            } else {
                newObj.request_item_quantity = array[i].request_item_quantity;
            }
            newRequest.push(newObj);
        }
        formType = requestType;
    }
    sendRequest(newRequest, formType);
}

function GetInputValue(num) {
    const rows = Array.from(table1.getElementsByTagName('tr')).slice(1);
    var newVal = "";
    for (var i = 0; i < rows.length; i++) {
        const firstCell = rows[i].cells[0];
        const textbox = firstCell.querySelector('input[type="text"]');
        if (i === num) {
            newVal = textbox.value;
            break;
        }
    }
    return newVal;
}

function sendRequest(jsonData,formType) {
    fetch("/Admin/Requisition?formType=" + formType, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(jsonData)
    })
        .then(response => {
            if (response.ok) {
                return response.text()
            }
            else {
                throw new Error('Network response was not ok');
            }
        })
        .then(data => {
            console.log('Response from server:', data);
            window.location.reload();
            localStorage.setItem("success", true);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
//populate table on html
function PopulateView() {
    viewData.length = 0;
    let uniqueIdentifier = {};
    for (var i = 0; i < allRequests.length; i++) {
        // Check if the current request ID is already in the uniqueIdentifier object
        if (!uniqueIdentifier[allRequests[i].Id]) {
            // If not, add the request to the viewData array and the uniqueIdentifier object
            viewData.push(allRequests[i]);
            uniqueIdentifier[allRequests[i].Id] = true;
        }
    }
}

function MaxRequestId() {
    const requestTypeIds = allRequests.map(request => request.request_type_id);
    const maxRequestTypeId = Math.max(...requestTypeIds);
    return maxRequestTypeId;
}

function UpdateStatus(idToFind, type) {
    filtered.length = 0;
    filterArray(idToFind);
    let edit = "";
    if (type == 'approve') {
        edit = "Approved";
        requestType = "approved";
    }
    else {
        edit = "Declined";
        requestType = "declined";
    }

    filtered.forEach(function (item) {
        item.request_status = edit;
    });
    setTemplate(filtered, "stats");
}