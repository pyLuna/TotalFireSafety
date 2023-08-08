console.log(data);

const ctx = document.getElementById('barchart').getContext('2d');
// Define a function to filter data by date range
function filterDataByDateRange(data, startDate, endDate) {
    return data.filter(function (item) {
        var date = new Date(item.Date);
        return date >= startDate && date <= endDate;
    });
}

// Get the start and end date from the input fields
var startDateInput = document.getElementById('startDate');
var endDateInput = document.getElementById('endDate');
var startDate = new Date(startDateInput.value);
var endDate = new Date(endDateInput.value);

// Filter the data by date range
var filteredData = filterDataByDateRange(data, startDate, endDate);


const barchart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: data.map(d => `${d.Name} (${d.Size ? d.Size : ""})`),
        datasets: [{
            label: 'Quantity',
            data: data.map(d => d.Quantity),
            backgroundColor: function (context) {
                var value = context.dataset.data[context.dataIndex];

                if (value > 500) {
                    return '#09AF10';
                }
                else if (value > 100) {
                    return '#FF932F';
                }

                else {
                    return '#FF0000';
                }
            },
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B',
        }]
    },
    options: {
      
        plugins: {
            legend: {
                display: false
            }
        },
        responsive: true,
        maintainAspectRatio: false,
        aspectRatio: 4 / 3,
        scales: {
            x: {
                padding: {
                    left: 50,
                    right: 50
                }
            },
            y: {
                beginAtZero: true
            }
        },
    }
});

const classSelect = document.getElementById('class');
const itemCategorySelect = document.getElementById('itemCategory');
const itemNameSelect = document.getElementById('itemName');

classSelect.addEventListener('change', updateChartData);
itemCategorySelect.addEventListener('change', function () {
    populateItemNames(); // populate item names based on selected category
    updateChartData();
});

itemNameSelect.addEventListener('change', updateChartData);

function populateItemNames() {
    const selectedCategory = itemCategorySelect.value;

    // Get all item names that belong to the selected category
    const itemNames = data.filter(d => d.Category === selectedCategory)
        .map(d => d.Name);

    // Remove existing options from the select element
    while (itemNameSelect.options.length > 0) {
        itemNameSelect.remove(0);
    }

    // Add new options to the select element
    for (let i = 0; i < itemNames.length; i++) {
        const option = document.createElement('option');
        option.text = itemNames[i];
        itemNameSelect.add(option);
    }
}

function updateChartData() {
    const selectedClass = classSelect.value;
    const selectedCategory = itemCategorySelect.value;
    const selectedName = itemNameSelect.value;

    let filteredData = data; // initialize filteredData to the original data array

    // Filter by item class
    if (selectedClass !== 'All') {
        filteredData = filteredData.filter(d => d.Class === selectedClass);
    }

    // Filter by item category
    if (selectedCategory !== 'Clear') {
        filteredData = filteredData.filter(d => d.Category === selectedCategory);
    }

    // Filter by item name
    if (selectedName !== '') {
        filteredData = filteredData.filter(d => d.Name === selectedName);
    }

    // Update the chart data and labels
    barchart.data.labels = filteredData.map(d => `${d.Name} (${d.Size ? d.Size : ""})`);
    barchart.data.datasets[0].data = filteredData.map(d => d.Quantity);

    // Update the chart colors based on the new data
    barchart.data.datasets[0].backgroundColor = function (context) {
        var value = context.dataset.data[context.dataIndex];

        if (value > 500) {
            return '#09AF10';
        } else if (value > 100) {
            return '#FF932F';
        } else {
            return '#FF0000';
        }
    };

    // Update the chart
    barchart.update();
}

// Populate item names based on selected category
populateItemNames();

startDateInput.addEventListener('change', function () {
    startDate = new Date(startDateInput.value);
    filteredData = filterDataByDateRange(data, startDate, endDate);
    barchart.data.labels = filteredData.map(d => `${d.Name} (${d.Size ? d.Size : ""})`);
    barchart.data.datasets[0].data = filteredData.map(d => d.Quantity);
    barchart.update();
});

endDateInput.addEventListener('change', function () {
    endDate = new Date(endDateInput.value);
    filteredData = filterDataByDateRange(data, startDate, endDate);
    barchart.data.labels = filteredData.map(d => `${d.Name} (${d.Size ? d.Size : ""})`);
    barchart.data.datasets[0].data = filteredData.map(d => d.Quantity);
    barchart.update();
});