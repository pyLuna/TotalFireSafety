//Global Options

const ctx3 = document.getElementById('barcharthori').getContext('2d');
const barcharthori = new Chart(ctx3, {
    type: 'bar',
    data: {
        labels: ['Pipe', 'Fire Extinguisher', 'Yellow', 'Green', 'Purple', 'Orange'],
        datasets: [{
            label: 'Item Quantity',
            data: [881, 700, 500, 420, 350, 100],
            backgroundColor: [
                '#2C74B3',
                '#5BC0F8',
                '#5BC0F8',
                '#5BC0F8',
                '#5BC0F8',
                '#5BC0F8'    
                
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
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});