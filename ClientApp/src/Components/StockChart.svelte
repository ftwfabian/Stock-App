<script>
  import { onMount } from 'svelte';
  import { createChart, ColorType } from 'lightweight-charts';
  import { Link, navigate } from 'svelte-routing';
  import { animationStore } from './stores.js'
  import FavoriteStar from './FavoriteStar.svelte';
  import ColorSpreadAnimation from './ColorSpreadAnimation.svelte'

  export let ticker = '';
  let isFavorite = false;
  $: chartRange = 21;
  let chartContainer;
  let chart;
  let currentSeries;

  let isAnimating = false;
  let animationColor = '';

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
      chart.applyOptions({ width, height: 270});
    }
  }

  function createChartInstance() {
    const { width, height } = chartContainer.getBoundingClientRect();
    chart = createChart(chartContainer, {
      width: width,
      height: 270,
      layout: {
        background: { type: ColorType.Solid, color: 'white' },
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

<main>
  <div class="chart-box">
      <h1>{ticker}</h1>
      <div class="chart-container">
          <div class="chart-area">
              <div class="chart-and-buttons">
                  <div bind:this={chartContainer} class="chart"></div>
              </div>
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

  <ColorSpreadAnimation color={animationColor} isActive={isAnimating}/>
</main>

<style>
  main {
      max-width: 1000px;
      margin: 0 auto;
      padding: 20px;
  }

  .chart-box {
      background-color: white;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
      margin-bottom: 20px;
  }

  h1 {
      margin-top: 0;
      margin-bottom: 15px;
      text-align: center;
  }

  .chart-container {
      width: 100%;
      margin: 0 auto;
  }

  .chart-area {
      width: 100%;
  }

  .chart-and-buttons {
      position: relative;
      width: 100%;
  }

  .chart {
      width: 100%;
      height: 250px;  /* Reduced height to make room for buttons */
  }

  .button-container {
      margin-top: 10px;
      display: flex;
      justify-content: flex-start;
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