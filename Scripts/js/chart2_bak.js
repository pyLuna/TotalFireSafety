var xValues = ['April','May','June','July','August','September','Octboer','November', 'December'];
var yValues = [20000,30000,50000,80000,100000,150000,180000,200000,250000];

const ctx2 = document.getElementById('scatter').getContext('2d');
const scatter = new Chart(ctx2, {
    type: "line",
  data: {
    labels: xValues,
    datasets: [{
      label:'Number of Sales',
      fill: false,
      lineTension: 0,
      backgroundColor: "#16215B",
      borderColor: "rgba(0,0,255,0.1)",
      data: yValues,      
    }]
  },
  options: {
    legend: {display: true},
    scales: {
      yAxes: [{ticks: {min: 1, max:12}}],
    }
  }
});