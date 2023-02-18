let jsonArray = [];
let filtered = [];
let fixedArray = [];
let name = document.querySelector('#name');
let contact = document.querySelector('#contact');
let empID = document.querySelector('#empId');
let dateDisplay = document.getElementById("dateDisplay");
let dateHired = document.getElementById("dateHired");
let username = document.getElementById("username");
let password = document.getElementById("password");
let position = document.getElementById("position");
let roles = document.getElementById("roles");
let rolesid = document.getElementById("rolesid");
let statsid = document.getElementById("statsid");
let stats = document.getElementById("stats");
let stats1 = document.getElementById("stats1");
let selroles = document.getElementById("sel-roles");
let selstats = document.getElementById("sel-stats");
let table = document.querySelector('#myTable tbody');
let form = document.getElementById("formId");
let credsid = document.getElementById("credsid");

function GetAllEmployeeInfo() {

    fetch('/Admin/SearchEmployee')
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

            //if (array[i].emp_name === null) {
            //    array[i].emp_name = '';
            //}
            //if (array[i].emp_position === null) {
            //    array[i].emp_position = '';
            //}
            //if (array[i].IsActive === null) {
            //    array[i].IsActive = 'InActive';

            //}
            //if (array[i].IsActive === 1) {
            //    array[i].IsActive = 'Active';
            //}
            let stats = "";
            if (array[i].Status?.IsActive === 1) {
                stats = "Active";
                if (array[i].Status?.IsLocked === 1) {
                    stats = "IsLocked";
                }
            }
            else {
                stats = "Inactive";
            }

            var row = `<tr>`;
            row += `<form action="/Admin/Users?item=${array[i].emp_no}" method="post" id="tableList">`;
            row += `<td id="emp_no"><label>${array[i].emp_no}</label></td><td name="emp_name"><label>${array[i].emp_name}</label></td><td name="emp_hiredDate"><label>${array[i].FormattedDate}</label></td><td name="emp_contact"><label>${array[i].emp_contact}</label></td><td name="emp_position"><label>${array[i].emp_position}</label><td name="IsActive"><label">${stats}</label>`;
            row += `</form>`;
            row += `<td id="hideActionBtn"><div class="user-action-style">`;
            row += ` <button class="edit-btn" title="EDIT SELECTED ITEM" onclick="openEditForm('${array[i].emp_no}')"> <a href="#"><span class="lar la-edit"></span></a></button>`;
            //row += `<button class="del-btn" title="DELETE SELECTED ITEM" onclick="canOpenPopup()"> <a href="#"><span class="lar la-trash-alt"></span></a></button>`;
            row += `</td></div>`;
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

    for (var i = 0; i < fixedArray.length; i++) {
        if (fixedArray[i].emp_no == value) {
            filtered.length = 0;
            filtered.push(fixedArray[i]);
        }
    }
    console.log(filtered);
    let statsValue = "";
    if (filtered[0].Status?.IsActive === 1) {
        stats = "Active";
        if (filtered[0].Status?.IsLocked === 1) {
            stats = "Locked";
        }
    }
    else {
        stats = "Inactive";
    }

    name.value = filtered[0]?.emp_name;
    contact.value = filtered[0]?.emp_contact;
    empID.value = filtered[0]?.emp_no;
    //dateHired.value = filtered[0]?.FormattedDate; //  TO Check
    username.value = filtered[0]?.Credential?.username;
    password.value = filtered[0]?.Credential?.password;
    position.value = filtered[0]?.emp_position;
    //roles.value = filtered[0].Role?.role1;
    //stats.value = statsValue;
    empID.value = filtered[0]?.emp_no;

    var dateString = new Date(filtered[0]?.FormattedDate);
    var dateUpdate = dateString.toLocaleDateString("en-US", { day: '2-digit', month: '2-digit', year: 'numeric' });
    var dateParts = dateUpdate.split("/");
    var date = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);

    dateDisplay.value = date.toISOString().substr(0, 10);

    selroles.selectedIndex = filtered[0]?.Role?.role1;

     //loop through all the options in the select element
    for (let i = 0; i < selstats.options.length; i++) {
        let option = selstats.options[i];
        // check if the option value exists in the valuesToCompare array
        if (stats === option.value) {
            selstats.selectedIndex = i;
        }
    }
}

//function exportArrayToCsv() {
//    let newArray = [['Employee ID', 'Name', 'Date Hired', 'Contact Number', 'Position']];

//    for (var i = 0; i < fixedArray.length; i++) {
//        newArray.push([fixedArray[i].emp_no, fixedArray[i].emp_name, fixedArray[i].emp_hiredDate, fixedArray[i].emp_contact, fixedArray[i].emp_position]);
//    }
//    var csv = newArray.map(row => row.join(',')).join('\n');
//    // Create a hidden link
//    var link = document.createElement('a');
//    link.setAttribute('href', 'data:text/csv;charset=utf-8,' + encodeURIComponent(csv));
//    link.setAttribute('download', 'Users.csv');

//    //// Trigger the download
//    link.click();
//}

//Descending
function SortDescending() {
    return fixedArray.sort((a, b) => {
        if (a['emp_name'] < b['emp_name']) return 1;
        if (a['emp_name'] > b['emp_name']) return -1;
        return 0;
    });
}

function Descend() {
    const result = SortDescending();
    setTable(result);
}

//Ascend
function SortAscending() {
    return fixedArray.sort((a, b) => {
        if (a['emp_name'] < b['emp_name']) return -1;
        if (a['emp_name'] > b['emp_name']) return 1;
        return 0;
    });
}

function Ascend() {
    const result = SortAscending();
    setTable(result);
}

function ResetForm() {
    var form = document.getElementById("formId");
    for (let i = 0; i < form.elements.length; i++) {
        form.elements[i].value = "";
    }
    selroles.selectedIndex = 0;
    selstats.selectedIndex = 0;
}

function setHiddenStats(index) {

    if (index === 1) {
        stats1.value = 1;
        stats.value = 0;
    }
    if (index === 2) {
        stats1.value = 0;
    }
    if (index === 3) {
        stats.value = 1
        stats1.value = 1;
    }
}

function submitForm() {
    roles.value = selroles.selectedIndex;
    rolesid.value = empID.value;
    stats.value = empID.value;
    credsid.value = empID.value;
    setHiddenStats(selstats.selectedIndex);
    form.submit();
}