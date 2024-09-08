<script>
  import { onMount } from 'svelte';
  import { createChart, ColorType } from 'lightweight-charts';
  import { Link, navigate } from 'svelte-routing';
  import FavoriteStar from './FavoriteStar.svelte';


  export let ticker = '';
  let isFavorite = false;
  $: chartRange = 21;
  let chartContainer;
  let chart;
  let currentSeries;

  $: if (chart && ticker && chartRange) {
    updateChartData();
  }

  async function fetchStockData(ticker) {
    const response = await fetch(`/api/StockApi/GetStockDataWithTimePeriod?ticker=${ticker}&timePeriod=${chartRange}`);
    const data = await response.json();

    return data.dates.map((date, index) => ({
      time: date.split('T')[0],
      value: data.prices[index]
    }));
  }

  function handleResize() {
    if (chart && chartContainer) {
      const { width, height } = chartContainer.getBoundingClientRect();
      chart.applyOptions({ width, height });
    }
  }

  function createChartInstance() {
    const { width, height } = chartContainer.getBoundingClientRect();
    chart = createChart(chartContainer, {
      width: width,
      height: height,
      layout: {
        background: { type: ColorType.Solid, color: '#ffffff' },
        textColor: '#333',
      },
      grid: {
        vertLines: {visible:false},
        horzLines: {visible:false}
      },
    });
  }

  async function updateChartData() {
    if (!chart) return;

    if (currentSeries) {
      chart.removeSeries(currentSeries);
    }

    currentSeries = chart.addLineSeries({
      color: '#2962FF',
      lineWidth: 2,
    });

    try {
      const data = await fetchStockData(ticker);
      currentSeries.setData(data);
      chart.timeScale().fitContent();
    } catch (error) {
      console.error('Error fetching or setting data:', error);
    }
  }

  function handleBuySellClick(e, action, color){
    e.preventDefault();
    navigate(`/Portal/${action}/${ticker}`);
  }

  onMount(() => {
    chartRange = 21;
    createChartInstance();
    updateChartData();

    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
      if (chart) chart.remove();
    };
  });

  function updateChartRange(range) {
    chartRange = range;
  }
</script>

<div class="chart-wrapper">
  <div class="chart-box">
    <div class="chart-container">
      <div class="chart-header stock-chart-header">
        <h1>{ticker}</h1>
        <Link 
          to="/Portal/buy/{ticker}" 
          class="btn btn-buy" 
          on:click={(e) => handleBuySellClick(e, 'buy', '#007bff')}
        >
          Buy
        </Link>
        <Link 
          to="/Portal/sell/{ticker}" 
          class="btn btn-sell" 
          on:click={(e) => handleBuySellClick(e, 'sell', '#dc3545')}
        >
          Sell
        </Link>
      </div>
      <div class="chart-area">
        <div class="chart-and-buttons">
          <div bind:this={chartContainer} class="chart"></div>
          <div class="button-container">
            <button class="chart-button" class:active={chartRange === 1} on:click={() => updateChartRange(1)}>1D</button>
            <button class="chart-button" class:active={chartRange === 5} on:click={() => updateChartRange(5)}>1W</button>
            <button class="chart-button" class:active={chartRange === 21} on:click={() => updateChartRange(21)}>1M</button>
            <button class="chart-button" class:active={chartRange === 251} on:click={() => updateChartRange(251)}>1Y</button>
            <FavoriteStar {isFavorite} {ticker}/>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  .chart-wrapper {
    display: flex;
    justify-content: center;
    width: 100%;
  }

  .chart-box {
    background-color: white;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    margin-bottom: 20px;
    width: 80%;
    max-width: 1000px;
  }

  .chart-container {
    width: 100%;
    margin: 0 auto;
    display: flex;
    align-items: flex-start;
  }

  .chart-area {
    flex-grow: 1;
  }

  .chart-and-buttons {
    position: relative;
    height: 180px;
  }

  .chart {
    width: 100%;
    height: 150px;
  }

  .button-container {
    position: absolute;
    bottom: 0;
    left: 0;
  }

  .chart-header {
    width: 110px;
    margin-right: 20px;
    display: flex;
    flex-direction: column;
    height: 180px;
  }

  .chart-header h1 {
    margin: 0;
    font-size: 3em;
    margin-bottom: 10px;
  }

  .stock-chart-header :global(.btn) {
    flex-grow: 1;
    margin-bottom: 5px;
    border: none;
    color: white;
    font-weight: bold;
    cursor: pointer;
    transition: background-color 0.3s ease;
    text-decoration: none;
    display: flex;
    align-items: center;
    justify-content: center;
    text-align: center;
    padding: 10px 0;
  }

  .stock-chart-header :global(.btn:last-child) {
    margin-bottom: 0;
  }

  .stock-chart-header :global(.btn-buy) {
    background-color: #007bff !important;
  }

  .stock-chart-header :global(.btn-buy:hover) {
    background-color: #0056b3 !important;
  }

  .stock-chart-header :global(.btn-sell) {
    background-color: #dc3545 !important;
  }

  .stock-chart-header :global(.btn-sell:hover) {
    background-color: #c82333 !important;
  }

  .chart-button {
    width: 40px;
    height: 30px;
    margin-right: 5px;
    background-color: #007bff;
    color: white;
    border: none;
    font-weight: bold;
    cursor: pointer;
    transition: background-color 0.3s ease;
  }

  .chart-button:hover {
    background-color: #0056b3;
  }

  .chart-button:last-child {
    margin-right: 0;
  }

  .chart-button.active {
    background-color: #0056b3;
  }
</style>