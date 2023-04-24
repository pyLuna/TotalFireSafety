//Global Options
var allItems = [];
var basecounts = [];
var ItemLabels = [];
var itemQuantity = [];
var itemSize = [];

//get the date from the label
function GetDate() {
    var start = localStorage.getItem("start");
    var end = localStorage.getItem("end");
    var diff = localStorage.getItem("diff");
    console.log(`start = ${start},,, end = ${end},,, diff = ${diff} `)
}

function extractNum(value) {
    let num = 0;
    let units = '';
    for (let i = 0; i < value?.length; i++) {
        if (!isNaN(parseInt(value[i]))) {
            num = num * 10 + parseInt(value[i]);
        } else if (value[i] !== ' ') {
            units += value[i];
        }
    }
    return { num, units };
}
//all base count items
function GetCounts() {
    fetch("/Admin/BaseResult")
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log(res.statusText);
            }
        })
        .then(data => {
            console.log(data);
        })
        .catch(error => {
            //window.location.replace('/Error/InternalServerError');
            console.error(error);
        });
}
// today's all item in inventory
function GetItems() {
    fetch("/Admin/ItemSummary")
        .then(res => {
            if (res.ok) {
                // API request was successful
                return res.json();
            } else {
                console.log(res.statusText);
            }
        })
        .then(data => {
            allItems.push(JSON.parse(data));
            PopulateDropdown(allItems);
            var results = PopulateLabels(allItems, "all");
            SetChart(results.label, results.quant);
        })
        .catch(error => {
            //window.location.replace('/Error/InternalServerError');
            console.error(error);
        });
}

function PopulateLabels(array,typeOf) {
    var label = [];
    var quant = [];
    if (typeOf === "all") {
        for (var i = 0; i < array[0].length; i++) {
            for (var ii = 0; ii < array[0][i].Items.length; ii++) {
                var size = array[0][i].Items[ii].Size;

                if (size == null || size == "null") {
                    size = "";
                }
                //ItemLabels.push(array[0][i].Items[ii].Name + " " + size);
                //itemQuantity.push(extractNum(array[0][i].Items[ii].Quantity).num);
                label.push(array[0][i].Items[ii].Name + " " + size);
                quant.push(extractNum(array[0][i].Items[ii].Quantity).num);
            }
        }
    }
    else {
        for (var i = 0; i < array.length; i++) {
                var size = array[i].Size;

                if (size == null || size == "null") {
                    size = "";
                }
                label.push(array[i].Name + " " + size);
                quant.push(extractNum(array[i].Quantity).num);
        }
    }
    return { label, quant };
}

function SetChart(labels,quantities) {
    //const ctx = document.getElementById('barchart').getContext('2d');
    const canvas = document.getElementById('barchart');
    const ctx = canvas.getContext('2d');

    // Get the chart instance
    const instance = Chart.getChart(canvas);

    // Check if the chart instance exists and destroy it
    if (instance) {
        instance.destroy();
    }

    const barchart = new Chart(ctx, {
        type: 'bar',
        data: {

            labels: labels,
            datasets: [{
                label: '',
                data: quantities,

                backgroundColor: function (context) {
                    var value = context.dataset.data[context.dataIndex];

                    if (value > 50) {
                        return '#09AF10';
                    }
                    else if (value > 20) {
                        return '#8F00FF';
                    }

                    else {
                        return '#FF0000';
                    }
                }
                ,
                borderWidth: 1,
                hoverBackgroundColor: '#fff',
                hoverBorderWidth: 1,
                hoverBorderColor: '#16215B',
            },
            ]
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
                },
                x: {
                    ticks: {
                        autoSkip: true, // Prevent auto-skipping of labels
                        //maxRotation: 90, // Rotate labels by 90 degrees
                        //minRotation: 90, // Rotate labels by 90 degrees
                        fontSize: 12, // Reduce font size of labels
                        padding: 10 // Add padding to labels
                    }
                }
            },
        }
    });
}