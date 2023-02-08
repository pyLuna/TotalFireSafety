//Global Options

const ctx = document.getElementById('barchart').getContext('2d');
const barchart = new Chart(ctx, {
    type: 'bar',
    data: {

        labels: ['Pipe', 'Fire Extinguisher', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: 'All Item Quantity',
            data: [500, 420, 881, 100, 80, 700],
            backgroundColor: function (context) {
                var value = context.dataset.data[context.dataIndex];
                if (value > 500) {
                    return '#96BB7C';
                }
                else if (value > 100) {
                    return '#FAD586';
                }

                else {
                    return '#C64756';
                }
            }
            ,
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B',
        },
        {
            label: 'Local Item Quantity',
            data: [700, 20, 80, 600, 200, 200],
            backgroundColor: function (context) {
                var value = context.dataset.data[context.dataIndex];
                if (value > 500) {
                    return '#96BB7C';
                }
                else if (value > 100) {
                    return '#FAD586';
                }

                else {
                    return '#C64756';
                }
            },
            borderWidth: 1,
            hoverBackgroundColor: '#fff',
            hoverBorderWidth: 1,
            hoverBorderColor: '#16215B',
        },
        {
            label: 'Imported Item Quantity',
            data: [700, 20, 80, 600, 200, 200],
            backgroundColor: function (context) {
                var value = context.dataset.data[context.dataIndex];
                if (value > 500) {
                    return '#96BB7C';
                }
                else if (value > 100) {
                    return '#FAD586';
                }

                else {
                    return '#C64756';
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

