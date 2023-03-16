const ctx = document.getElementById('barchart').getContext('2d');
const barchart = new Chart(ctx, {
    type: 'bar',
    data: {

        labels: ['Pipe', 'Fire Extinguisher', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: '',
            data: [500, 420, 881, 100, 80, 700],

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
            }
            ,
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B',
        },
        {
            label: '',
            data: [700, 20, 80, 600, 200, 200],
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
        },
        {
            label: '',
            data: [700, 20, 80, 600, 200, 200],
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
            }
        },
    }
});