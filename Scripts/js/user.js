let jsonArray = [];
let filtered = [];

//get all the employees from MVC Controller
function GetAll() {

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
            console.log(data);
            //localStorage.setItem('data', JSON.stringify(data)); //save result to a local storage for fast retrieving of data
            jsonArray.push(data);
            DisplayAllEmployee();
        })
        .catch(error => {
            console.error(error);
        });
}
//display all empployee
function DisplayAllEmployee() {
    for (var i = 0; i < jsonArray[0].length; i++) {
        filtered.length = 0;
        filtered.push(jsonArray[0][i]);
        setTable(filtered);
    }
}
//set the table value
function setTable(array) {
    const table = document.querySelector('#myTable tbody');
    if (array.length != 0) { // display all data from array if the Search textbox is clear

        array.forEach(item => {
            // append this row to html table
            const row = `<tr><td><input type="checkbox"></td><td>${item.emp_no}</td><td>${item.emp_name}</td><td>${item.emp_hiredDate}</td><td>${item.emp_contact}</td><td>${item.emp_position}</td></tr>`;
            table.innerHTML += row;
        });
    }
    else {
        //error handler if input value not found
        table.innerHTML = " ";
        const errorMessageRow = document.createElement('tr');
        errorMessageRow.style.textAlign = "center";
        errorMessageRow.style.fontStyle = "italic";
        errorMessageRow.innerHTML = "<td colspan='6'>Employee Not found<td>";
        //console.log(res.statusText);
        table.appendChild(errorMessageRow);
    }

}
//filter array for search function
function filterArray(value) {
    filtered.length = 0;
    for (var j = 0; j < jsonArray[0].length; j++) {
        if (JSON.stringify(jsonArray[0][j]).toLowerCase().includes(value)) {
            filtered.push(jsonArray[0][j]);
        }
    }
}
//method that will call by the onkeyup() event
function FindEmployee() {
    //jsonArray.length = 0;
    //jsonArray.push(JSON.parse(localStorage.getItem('data')));

    const table = document.querySelector('#myTable tbody');
    const value = document.getElementById('employeeSearch').value;

    filterArray(value.toLowerCase());
    table.innerHTML = ' ';
    setTable(filtered);
}





