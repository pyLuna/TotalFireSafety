//Global Options

var summ = [];

const radioButtons = document.querySelectorAll('input[name="list"]');
radioButtons.forEach(radioButton => {
    radioButton.addEventListener("change", () => {
        change(radioButton.value);
        // Code to execute when radio button changes
    });
});

function PopulateLabels1(array, typeOf){
    var label = [];
    var quant = [];
    for (var i = 0; i < array.length; i++) {
        var size = array[i].Items.Size;

        if (size == null || size == "null") {
            size = "";
        }
        label.push(array[i].Items.Name + " " + size);
        quant.push(extractNum(array[i].Items.Quantity).num);
    }
    return { label, quant };
}

function change(value) {
    let arr = [];

    if (value == "all") {
        var results = PopulateLabels(summ, "other");
        SetChart1(results.label, results.quant);
        return;
    }

    for (var i = 0; i < summ[0].length; i++) {
        if (summ[0][i].TotalRequest > summ[0][i].Average && value == "fast") {
            arr.push(summ[0][i]);
        }
        else if (summ[0][i].TotalRequest < summ[0][i].Average && value == "slow") {
            arr.push(summ[0][i]);
        }
    }
    console.log(arr);
    var results = PopulateLabels1(arr, "filtered");
    SetChart1(results.label, results.quant);
}

function GetSupplies() {
    fetch(`/Admin/ItemSupplies`)
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log(res.statusText);
            }
        })
        .then(data => {
            summ.length = 0;
            summ.push(data);
            summ.sort((a, b) => {
                const quantityA = extractNum(a.TotalQuantity).num;
                const quantityB = extractNum(b.TotalQuantity).num;

                if (quantityA < quantityB) {
                    return -1;
                }
                if (quantityA > quantityB) {
                    return 1;
                }
                return 0;
            });
            console.log(summ);
            var results = PopulateLabels(summ, "other");
            SetChart1(results.label, results.quant);
        })
        .catch(error => {
            //window.location.replace('/Error/InternalServerError');
            console.error(error);
        });
}

function SetChart1(labels, quantities) {
    const canvas = document.getElementById('barcharthori');
    const ctx3 = canvas.getContext('2d');

    const instance = Chart.getChart(canvas);

    // Check if the chart instance exists and destroy it
    if (instance) {
        instance.destroy();
    }
    // Check if labels and quantities are null
    if (labels.length == 0 || quantities.length == 0) {
        ctx3.clearRect(0, 0, canvas.width / 2, canvas.height); // Clear the canvas
        ctx3.font = '20px Arial'; // Set font size and type
        ctx3.fillStyle = '#000000'; // Set text color
        ctx3.textAlign = 'center'; // Set text alignment to center
        ctx3.fillText('No data', canvas.width / 2, canvas.height / 2); // Display "No data" message in the center of the canvas
        return;
    }
    const barcharthori = new Chart(ctx3, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Quantity',
                data: quantities,
                backgroundColor: [
                    '#09AF10'

                ],
                borderColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderWidth: 6, // Increase line width
                pointRadius: 4, // Increase point radius
                pointBackgroundColor: '#16215B', // Set point background color
                pointBorderColor: '#16215B', // Set point border color
                pointBorderWidth: 2, // Set point border width
                pointHoverRadius: 6, // Increase point hover radius
                pointHoverBackgroundColor: '#16215B', // Set point hover background color
                pointHoverBorderColor: '#fff', // Set point hover border color
                pointHoverBorderWidth: 2, // Set point hover border width
                //tension: 0.4 // Set curve tension
                //borderWidth: 1,
                //hoverBackgroundColor: '#fff',
                //hoverBorderWidth: 1,
                //hoverBorderColor: '#16215B'


            }]
        },
        options: {
            plugins: {
                legend: {
                    display: false
                }
            },
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            aspectRatio: 4 / 3,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

