// portfolio.js

//Charts...
function fetchPortfolioHoldings(portfolioId) {

    return fetch(`/api/PortfolioApi/GetPortfolioHoldings?portfolioId=${portfolioId}`)
        .then(response => response.json());
       
}

function fetchPortfolioCanvasData(portfolioId, days) {

    fetch(`/api/PortfolioApi/GetPortfolioCanvasData?portfolioId=${portfolioId}&days=${days}`)
        .then(response => response.json())
        .then(data => {
            updatePortfolioCanvas(data.prices, data.dates, data.portfolioName, portfolioId);
        })
}

function fetchStockCanvasData(ticker, days) {
    return fetch(`/api/PortfolioApi/GetStockCanvasData?ticker=${ticker}&days=${days}`)
        .then(response => response.json());
}

function updatePortfolioCanvas(prices, dates, portfolioName, portfolioId) {
    var portfolioNameHeader = document.getElementById('portfolioNameHeader');
    portfolioNameHeader.textContent = portfolioName;

    var canvasContainer = document.getElementById('portfolioCanvasContainer');
    canvasContainer.innerHTML = '';
    canvasContainer.style.textAlign = 'center';
    var oldCanvas = document.getElementById('portfolioCanvas');
 
    if (oldCanvas) {
        canvasContainer.removeChild(oldCanvas);
    }
   
    var newCanvas = document.createElement('canvas');
    newCanvas.id = 'portfolioCanvas';
    newCanvas.className = 'stockchart-canvas';
    newCanvas.width = 100;
    newCanvas.height = 30;
    
    canvasContainer.appendChild(newCanvas);

    //start making changes
    var buttonContainer = document.createElement('div');
    buttonContainer.style.textAlign = 'left';
    buttonContainer.className = 'button-container';
    ['1D', '1W', '1M', '1Y'].forEach((range, index) => {
        var button = document.createElement('button');
        button.className="btn btn-primary"
        button.textContent = range;
        if (index == 0) {
            button.style.marginLeft = '60px';
        } else {
            button.style.marginLeft = '5px';
        }

        button.addEventListener('click', function (event) {
            if (event.target.tagName == 'BUTTON') {
                var days;
                switch (range) {
                    case '1D':
                        days = 1;
                        break;
                    case '1W':
                        days = 7;
                        break;
                    case '1M':
                        days = 30;
                        break;
                    case '1Y':
                        days = 365;
                        break;
                }
                
                fetchPortfolioCanvasData(portfolioId, days);
            }
        });
        buttonContainer.appendChild(button);
    });
    canvasContainer.appendChild(buttonContainer);
    //end making changes

    var ctx = document.getElementById('portfolioCanvas').getContext('2d');
    var chart = new Chart(ctx, populateCanvas("portfolio", "portfolioValue", dates, prices));

}

function updateHoldingsCanvases(holdings) {
    console.log(holdings);

    var holdingsContainer = document.getElementById('holdingsContainer');
    holdingsContainer.innerHTML = '';

    // Sort holdings by value descending
    holdings.sort((a, b) => b.value - a.value);

    // Create a container for the current holding
    var currentHoldingContainer = document.createElement('div');
    currentHoldingContainer.id = 'currentHoldingContainer';
    holdingsContainer.appendChild(currentHoldingContainer);

    // Set the current index in the DOM
    holdingsContainer.dataset.currentIndex = '0';

    // Initial display
    displayHolding(0);

    function navigateHoldings(direction) {
        let currentIndex = parseInt(holdingsContainer.dataset.currentIndex);
        let newIndex = currentIndex + direction;

        if (newIndex >= 0 && newIndex < holdings.length) {
            holdingsContainer.dataset.currentIndex = newIndex.toString();
            displayHolding(newIndex);
        }
    }

    function displayHolding(index) {
        let holding = holdings[index];
        currentHoldingContainer.innerHTML = '';

        var holdingElement = document.createElement('div');
        holdingElement.className = 'stock-container';

        var titleElement = document.createElement('h4');
        titleElement.className = 'centered-header';
        titleElement.textContent = `${holding.ticker} - Quantity: ${holding.quantity}`;
        holdingElement.appendChild(titleElement);

        // Create navigation buttons
        var navigationContainer = document.createElement('div');
        navigationContainer.className = 'navigation-container';
        navigationContainer.style.textAlign = 'center';
        navigationContainer.style.marginBottom = '10px';
        
        var prevButton = document.createElement('button');
        prevButton.textContent = 'Previous';
        prevButton.className = 'btn btn-primary';
        prevButton.onclick = () => navigateHoldings(-1);
        prevButton.disabled = index === 0;
        console.log("Previous Button Disabled: ", prevButton.disabled);

        var nextButton = document.createElement('button');
        nextButton.textContent = 'Next';
        nextButton.className = 'btn btn-primary';
        nextButton.style.marginLeft = '10px';
        nextButton.onclick = () => navigateHoldings(1);
        nextButton.disabled = index === holdings.length - 1;
        console.log("Next Button Disabled: ", nextButton.disabled);

        navigationContainer.appendChild(prevButton);
        navigationContainer.appendChild(nextButton);
        holdingElement.appendChild(navigationContainer);

        var stockChartElement = document.createElement('div');
        stockChartElement.className = 'stock-chart';

        var canvasContainer = document.createElement('div');
        canvasContainer.className = 'canvas-container';
        canvasContainer.id = `canvasContainer_${holding.ticker}`;
        holdingElement.appendChild(canvasContainer);
        var canvas = document.createElement('canvas');
        canvas.className = 'stockchart-canvas';
        canvas.width = 100;
        canvas.height = 30;
        canvasContainer.appendChild(canvas);
        holdingElement.appendChild(canvasContainer);

        var buttonContainer = document.createElement('div');
        buttonContainer.style.textAlign = 'left';
        buttonContainer.className = 'button-container';
        ['1D', '1W', '1M', '1Y'].forEach((range, index) => {
            var button = document.createElement('button');
            button.className = "btn btn-primary";
            button.textContent = range;
            button.style.marginLeft = index === 0 ? '60px' : '5px';
            
            button.addEventListener('click', function (event) {
                if (event.target.tagName == 'BUTTON') {
                    var days = range === '1D' ? 1 : range === '1W' ? 7 : range === '1M' ? 30 : 365;
                    fetchStockCanvasData(holding.ticker, days)
                        .then(data => {
                            updateStockCanvas(canvasContainer, data.prices, data.dates, holding.ticker);
                        });
                }
            });
            buttonContainer.appendChild(button);
        });
        stockChartElement.appendChild(buttonContainer);
        holdingElement.appendChild(stockChartElement);

        currentHoldingContainer.appendChild(holdingElement);

        // Fetch initial data for 30 days
        fetchStockCanvasData(holding.ticker, 30)
            .then(data => {
                updateStockCanvas(canvasContainer, data.prices, data.dates, holding.ticker);
            });
    }
}

function updateStockCanvas(canvasContainer, prices, dates, ticker) {

    var oldCanvas = canvasContainer.querySelector('canvas');
    if (oldCanvas) {
        canvasContainer.removeChild(oldCanvas);
    }

    var newCanvas = document.createElement('canvas');
    newCanvas.className = 'stockchart-canvas';
    newCanvas.width = 100;
    newCanvas.height = 30;
    canvasContainer.appendChild(newCanvas);
    var ctx = newCanvas.getContext('2d');
    var chart = new Chart(ctx, populateCanvas("stock", ticker,dates,prices));
    
}
     


function populateCanvas(canvasType, ticker, dates, prices) { 
    if (canvasType === "stock") {
        return {
            type: 'line',
            data: {
                labels: dates,
                datasets: [{
                    label: ticker,
                    data: prices,
                    tension: 0.1,
                    //cubicInterpolationMode: 'monotone',
                    backgroundColor: 'rgba(0, 0, 192, 0.5)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    fill: 'origin',
                    borderWidth: 2,
                    stepped: false
                }]
            },
            options: {
                animation:false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            callback: function (value, index, values) {
                                if (index % 1000 == 999) {
                                    var date = new Date(this.getLabelForValue(value));
                                    return date.toLocaleDateString();
                                }
                                return '';
                            }
                        }
                    }
                }
            }
        };
    }
    if (canvasType = "portfolio") {
        return{
            type: 'line',
                data: {
                labels: dates,
                    datasets: [{
                        label: ticker,
                        data: prices,
                        tension: 0.1,
                        //cubicInterpolationMode: 'monotone',
                        backgroundColor: 'rgba(0, 0, 192, 0.5)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        fill: 'origin',
                        borderWidth: 2,
                        stepped: false
                    }]
            },
            options: {
                animation:false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            callback: function (value, index, values) {
                                if (index % 1000 == 999) {
                                    var date = new Date(this.getLabelForValue(value));
                                    return date.toLocaleDateString();
                                }
                                return '';
                            }
                        }
                    }
                }
            }



        }
    }
}


document.addEventListener('DOMContentLoaded', function () {
    
    var portfolioSelect = document.getElementById('portfolioSelect');
    portfolioSelect.addEventListener('change', (event) => {
        //var portfolioId = this.value;
        var portfolioId = event.target.value;
                   
        fetchPortfolioCanvasData(portfolioId, 30);
        fetchPortfolioHoldings(portfolioId)
            .then(data => {
               
                updateHoldingsCanvases(data.holdings);
            }); 
    });

    // Trigger the change event on page load to load the initial portfolio holdings
    setTimeout(() => {
        portfolioSelect.dispatchEvent(new Event('change'));
    }, 100);
});

//...End Charts

