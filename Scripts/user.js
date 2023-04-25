let jsonArray = [];
let filtered = [];
let fixedArray = [];
let selroles = document.getElementById("sel_roles");
let selstats = document.getElementById("sel_stats");
let table = document.querySelector('#myTable tbody');
let form = document.getElementById("formId");
let credsid = document.getElementById("credsid");
let hidRole = document.getElementById('emp_role');

selroles.addEventListener("change", function () {

    hidRole.value = selroles.selectedIndex;
});
selstats.addEventListener("change", function () {
    let hidAct = document.getElementById('emp_act');
    let hidLock = document.getElementById('emp_lock');

    if (selstats.selectedIndex == 1) {
        hidAct.value = 1;
        hidLock.value = 0;
    }
    else if (selstats.selectedIndex == 2) {
        hidAct.value = 0;
        hidLock.value = 0;
    }
    else {
        hidAct.value = 0;
        hidLock.value = 1;
    }

});

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
                errorMessageRow.innerHTML = '<td colspan="10">Loading Error<td>';
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
    if (array.length != 0) {
        for (var i = 0; i < array.length; i++) {

            //let stats = "";
            //if (array[i].Status?.IsActive === 1) {
            //    stats = "Active";
            //    if (array[i].Status?.IsLocked === 1) {
            //        stats = "Locked";
            //    }
            //}
            //else {
            //    stats = "Inactive";
            //}

            let stats = "";
            let role = "";

            if (array[i].Status.IsActive === 1 && array[i].Status.IsUser === 1) {
                stats = "Active";
            }
            if (array[i].Status.IsActive === 0 && array[i].Status.IsUser === 1){
                stats = "Inactive"
            }
            if (array[i].Status?.IsLocked === 1 && array[i].Status.IsUser === 1) {
                stats = "Locked"
            }
            if (array[i].Status.IsUser === 0) {
                role = "null";
                stats = "Nonuser";
            }
            if (array[i].Role?.role1 === 1) {
                role = "Admin";
            }
            else if (array[i].Role?.role1 === 2) {
                role = "Warehouse Admin";
            }
            else if (array[i].Role?.role1 === 3) {
                role = "Office Admin";
            }


            let mname = "";
            if (array[i].emp_name == null) {
                mname = "";
            }
            else {
                mname = array[i].emp_name;
            }

            let style = "";
            if (stats.toLowerCase() == "active") {
                style = "stat-appr";
            } else if (stats.toLowerCase() == "inactive") {
                style = "stat-dec";
            }
            else if (stats.toLowerCase() == "locked") {
                style = "stat-in";
            }
            else {
                style = "stat-dec";
            }

            var row = `<tr>`;
            row += `<td id="emp_no"><label>${array[i].emp_no}</label></td>`;
            row += `<td><label>${array[i].emp_lname}</label></td>`;
            row += `<td><label>${array[i].emp_fname}</label></td>`;
            row += `<td><label>${mname}</label></td>`;
            row += `<td><label>${array[i].FormattedDate}</label></td>`;
            row += `<td><label>${array[i].emp_contact}</label></td>`;
            row += `<td><label>${array[i].emp_position}</label>`;
            row += `<td ><label class="${style}">${stats}</label>`;
            row += `<td><label>${role}</label></td>`;
            row += `<td id="hideActionBtn"><div class="user-action-style">`;
            row += ` <button class="edit-btn" id="edit-btn-users1" title="EDIT SELECTED ITEM" onclick=" openEditForm('${array[i].emp_no}')">
            <a href="#"><span class="lar la-edit"></span></a></button>`;
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
        errorMessageRow.innerHTML = "<td colspan='10'>User Not found<td>";
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

function FilterByActive(value) {
    //filtered.length = 0;
    let arr = [];
    for (var j = 0; j < fixedArray.length; j++) {
        if (JSON.stringify(fixedArray[j].Status.IsActive).toLowerCase().includes(value)) {
            arr.push(fixedArray[j]);
        }
    }
    return arr;
}

function FilterByLock(value) {
    let arr = [];
    for (var j = 0; j < fixedArray.length; j++) {
        if (JSON.stringify(fixedArray[j].Status.IsLocked).toLowerCase().includes(value)) {
            arr.push(fixedArray[j]);
        }
    }
    return arr;
}

function FilterFunc(index) {
    let num = 0;
    let catcher = [];
    catcher.length = 0;
    if (index == 'active') {
        num = 1;
        catcher = FilterByActive(num);
    }
    else if (index == 'inactive') {
        num = 0;
        catcher = FilterByActive(num);
    }
    else {
        num = 1;
        catcher = FilterByLock(num);
    }
    setTable(catcher);
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

    let fname = document.getElementById('emp_Fname');
    let lname = document.getElementById('emp_Sname');
    let mname = document.getElementById('emp_Mname');
    let id = document.getElementById('emp_id');
    let hiredDate = document.getElementById('hiredDate');
    let emp_contact = document.getElementById('emp_contact');
    let uname = document.getElementById('Credential.username');
    let pword = document.getElementById('Credential.password');
    let posi = document.getElementById('posi');

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

    fname.value = filtered[0].emp_fname;
    lname.value = filtered[0].emp_lname;
    mname.value = filtered[0].emp_name ? filtered[0].emp_name : "";
    id.value = filtered[0].emp_no;
    emp_contact.value = filtered[0].emp_contact;
    uname.value = filtered[0].Credential.username;
    pword.value = filtered[0].Credential.password;
    posi.value = filtered[0].emp_position;
    
    var dateString = new Date(filtered[0]?.FormattedDate);
    var dateUpdate = dateString.toLocaleDateString("en-US", { day: '2-digit', month: '2-digit', year: 'numeric' });
    var dateParts = dateUpdate.split("/");
    var date = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);

    hiredDate.value = date.toISOString().substr(0, 10);

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

// General sort function
function SortDescending(array, itemToSort) {
    return array.sort((a, b) => {
        if (a[itemToSort] < b[itemToSort]) return 1;
        if (a[itemToSort] > b[itemToSort]) return -1;
        return 0;
    });
}
function SortAscending(array, itemToSort) {
    return array.sort((a, b) => {
        if (a[itemToSort] < b[itemToSort]) return -1;
        if (a[itemToSort] > b[itemToSort]) return 1;
        return 0;
    });
}
function Sort(item, value) {
    let iElement = item.querySelector('i');
    let arrayToSend = [];
    let catcher = [];

    if (filtered.length != 0) {
        arrayToSend = filtered;
    }
    else {
        arrayToSend = fixedArray;
    }

    if (iElement.classList.contains('la-sort-down')) {
        iElement.classList.remove('la-sort-down');
        iElement.classList.add('la-sort-up');
        catcher = SortAscending(arrayToSend, value);
    } else {
        iElement.classList.remove('la-sort-up');
        iElement.classList.add('la-sort-down');
        catcher = SortDescending(arrayToSend, value);
    }
    setTable(catcher);
}

function ResetForm() {
    var form = document.getElementById("formId");
    for (let i = 0; i < form.elements.length; i++) {
        form.elements[i].value = "";
    }
    selroles.selectedIndex = 0;
    selstats.selectedIndex = 0;
}

function submitForm() {
    let userField = document.getElementById("emp_user");
    let rl = selroles.selectedIndex;

    if (rl != 1 && rl != 2 && rl != 3) {
        userField.value = 0;
    }
    else {
        userField.value = 1;
    }

    var bool = checkForm();
    if (bool) {
        form.submit();
    }
}