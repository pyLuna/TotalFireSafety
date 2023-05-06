//Global Options
let projs = [];
let names = [];
let projName = [];
let progs = [];
//all base count items
function GetProjects() {
    fetch(`/Admin/AllProjects`)
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log(res.statusText);
            }
        })
        .then(data => {
            projs.length = 0;
            projs.push(data);
            var result = PopulateLabels2(projs);
            result = SetChart2(result.labels, result.quant);
        })
        .catch(error => {
            //window.location.replace('/Error/InternalServerError');
            console.error(error);
        });
}

function PopulateLabels2(array) {
    var labels = [];
    var quant = [];
    for (var i = 0; i < array.length; i++) {
        for (var ii = 0; ii < array[0][i].Projects.length; ii++) {
            names.push(array[0][i].Projects[ii].Lead_Name);
            labels.push(array[0][i].Projects[ii].Name);
            var progress = array[0][i].Projects[ii].Reports / array[0][i].Projects[ii].WorkingDays * 100;
            progs.push(Math.round(progress));
            quant.push(Math.round(progress));
        }
    }
    return { labels, quant };
}

function SetChart2(labels, quantities) {
    //const ctx4 = document.getElementById('barcharthorii').getContext('2d');
    const canvas = document.getElementById('barcharthorii');
    const ctx4 = canvas.getContext('2d');

    // Get the chart instance
    const instance = Chart.getChart(canvas);

    // Check if the chart instance exists and destroy it
    if (instance) {
        instance.destroy();
    }
    // Check if labels and quantities are null
    if (labels.length == 0 || quantities.length == 0) {
        ctx4.clearRect(0, 0, canvas.width / 2, canvas.height); // Clear the canvas
        ctx4.font = '20px Arial'; // Set font size and type
        ctx4.fillStyle = '#000000'; // Set text color
        ctx4.textAlign = 'center'; // Set text alignment to center
        ctx4.fillText('No data', canvas.width / 2, canvas.height / 2); // Display "No data" message in the center of the canvas
        return;
    }
const barcharthorii = new Chart(ctx4, {
    type: 'bar',
    data: {
        labels: labels,
        datasets: [{
            label: 'Progress',
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
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B'
            

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
            },
            x: {
                min: 0, // Set the minimum value for the x-axis
                max: 100, // Set the maximum value for the x-axis
                ticks: {
                    stepSize: 10 // Set the step size for the x-axis
                }
            }
        }
    }
});
}