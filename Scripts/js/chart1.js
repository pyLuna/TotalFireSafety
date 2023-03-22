console.log(data);

const ctx = document.getElementById('barchart').getContext('2d');



const barchart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: data.map(d => d.Name),
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
            y: {
                beginAtZero: true
            }
        },
    }
});

const classSelect = document.getElementById('class');
const itemCategorySelect = document.getElementById('itemCategory');
const itemNameInput = document.getElementById('itemName');

classSelect.addEventListener('change', updateChartData);
itemCategorySelect.addEventListener('change', function () {
    itemNameInput.value = ''; // clear itemNameInput value
    updateChartData();
});
itemNameInput.addEventListener('change', updateChartData);

function updateChartData() {
    const selectedClass = classSelect.value;
    const selectedCategory = itemCategorySelect.value;
    const selectedName = itemNameInput.value;

    let filteredData = data;

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
    barchart.data.labels = filteredData.map(d => d.Name);
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