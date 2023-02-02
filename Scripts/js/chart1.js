//Global Options

const ctx = document.getElementById('barchart').getContext('2d');
const barchart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['Pipe', 'Fire Extinguisher', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: 'Item Quantity',
            data: [500, 420, 881, 100, 350, 700],
            backgroundColor: [
                '#16215B'
                
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
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});