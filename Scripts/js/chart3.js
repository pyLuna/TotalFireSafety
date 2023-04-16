//Global Options

const ctx3 = document.getElementById('barcharthori').getContext('2d');
const barcharthori = new Chart(ctx3, {
    type: 'line',
    data: {
        labels: ['Pipe', 'Fire Extinguisher', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: 'Item Quantity',
            data: [881, 700, 500, 420, 350, 100],
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

