//Global Options

const ctx4 = document.getElementById('barcharthorii').getContext('2d');
const barcharthorii = new Chart(ctx4, {
    type: 'bar',
    data: {
        labels: ['Starcross', 'Three SL', 'Uber SM Aura', 'KBC Tower 1', 'Kolora Inc', 'Molito Narathai - Alabang', 'Triump', 'Malvar Enerzon'],
        datasets: [{
            label: 'Item Quantity',
            data: [40, 80, 10, 70, 50, 30, 50, 60, 90, 100,],
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
            }
        }
    }
});

