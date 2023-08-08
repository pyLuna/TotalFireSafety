console.log(data1);

// Get the canvas element and context
var ctx5 = document.getElementById('charta').getContext('2d');

// Create the chart
var charta = new Chart(ctx5, {
    type: 'line',
    data: {
        labels: data1.map(function (item) {
            return item.Name1;
        }),
        datasets: [{
            label: 'Item Quantity',
            data: data1.map(function (item) {
                return item.Quantity1;
            }),
            backgroundColor: data1.map(function (item) { // Set background color based on Quantity1
                return item.Quantity1 >= 51 ? '#09AF10' : '#FF932F';
            }),
            borderColor: data1.map(function (item) { // Set border color based on Quantity1
                return item.Quantity1 >= 51 ? '#09AF10' : '#FF932F';
            }),
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B'
        }]
    },
    options: {
        responsive: true,
        interaction: {
            mode: 'index',
            intersect: false,
        },
        stacked: false,
        plugins: {
            legend: {
                display: false
            },

          
        },
        scales: {
            y: {
                type: 'linear',
                display: true,
                position: 'left',
            },
            y1: {
                type: 'linear',
                display: true,
                position: 'right',

                // grid line settings
                grid: {
                    drawOnChartArea: false, // only want the grid lines for one axis to show up
                },
            },
        }
    },

});

var select = document.getElementById('moving');

// Add event listener to select tag
select.addEventListener('change', function () {
    var selectedOption = this.options[this.selectedIndex].id;
    var filteredData;

    // Filter data based on selected option
    if (selectedOption === 'fast') {
        filteredData = data1.filter(function (item) {
            return item.Quantity1 >= 51;
        });
    } else if (selectedOption === 'slow') {
        filteredData = data1.filter(function (item) {
            return item.Quantity1 <= 50;
        });
    } else {
        filteredData = data1;
    }

    // Update chart data and labels
    charta.data.labels = filteredData.map(function (item) {
        return item.Name1;
    });
    charta.data.datasets[0].data = filteredData.map(function (item) {
        return item.Quantity1;
    });

    // Update chart colors
    charta.data.datasets[0].backgroundColor = filteredData.map(function (item) {
        return item.Quantity1 >= 51 ? '#09AF10' : '#FF932F';
    });
    charta.data.datasets[0].borderColor = filteredData.map(function (item) {
        return item.Quantity1 >= 51 ? '#09AF10' : '#FF932F';
    });

    charta.update();
});