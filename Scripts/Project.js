// Get the input table and the output table
const inputTable = document.querySelector('.input-mems-list table');
const outputTable = document.querySelector('.add-new-power table');

// Get the Save button and attach an event listener to it
const saveButton = document.querySelector('.input-mems-main button:first-of-type');
saveButton.addEventListener('click', () => {
    // Get the name and position input from the first table
    const nameInput = inputTable.querySelector('tbody tr:first-of-type td:first-of-type');
    const positionInput = inputTable.querySelector('tbody tr:first-of-type td:last-of-type');

    // Create a new row in the output table with the same values as the input table
    const newRow = document.createElement('tr');
    const nameCell = document.createElement('td');
    nameCell.innerHTML = nameInput.innerHTML;
    newRow.appendChild(nameCell);

    const positionCell = document.createElement('td');
    positionCell.innerHTML = positionInput.innerHTML;
    newRow.appendChild(positionCell);

    outputTable.querySelector('tbody').appendChild(newRow);

    // Clear the input fields
    nameInput.innerHTML = '';
    positionInput.innerHTML = '';
});