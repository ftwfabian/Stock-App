<script>
  import { onMount } from 'svelte';
  import StockChart from "../Components/StockChartBuySell.svelte";
  import StockSearch from '../Components/StockSearch.svelte';
  import StockSort from '../Components/StockSort.svelte';

  export let firstTicker;
  export let key;
  let defaultTickers = [];
  let searchedTicker = '';
  let isLoading = false;
  let prevFirstTicker = null;

  $:{
    console.log('firstTicker changed:', firstTicker);
    fetchSortedTickers('marketcap', 'desc');
  }
  

  $: if (firstTicker !== prevFirstTicker) {
      prevFirstTicker = firstTicker;
      reorderTickers();
  }

  async function fetchSortedTickers(sortCriteria, sortOrder) {
      isLoading = true;
      try {
          const url = new URL('/api/StockApi/GetSortedTickers', window.location.origin);
          url.searchParams.append('sortCriteria', sortCriteria);
          url.searchParams.append('sortOrder', sortOrder);
          const response = await fetch(url);

          if (!response.ok) {
              throw new Error("Network Response was not Ok");
          }

          defaultTickers = await response.json();
          reorderTickers();
      } catch (error) {
          console.error('There was a problem fetching the tickers', error);
      } finally {
          isLoading = false;
      }
  }

  function reorderTickers() {
      if (!defaultTickers.length) return;

      const firstTickerIndex = defaultTickers.findIndex(ticker => ticker === firstTicker);

      if (firstTickerIndex !== -1) {
          // If firstTicker is found, move it to the beginning
          defaultTickers = [
              defaultTickers[firstTickerIndex],
              ...defaultTickers.slice(0, firstTickerIndex),
              ...defaultTickers.slice(firstTickerIndex + 1)
          ];
      } else {
          // If firstTicker is not found, add it to the beginning
          defaultTickers = [firstTicker, ...defaultTickers];
      }
  }

  onMount(async () => {
      const [sortCriteria, sortOrder] = ['marketcap', 'desc'];
      await fetchSortedTickers(sortCriteria, sortOrder);
  });

  function handleSearch(event) {
      searchedTicker = event.detail;
  }

  function handleSort(event) {
      const [sortCriteria, sortOrder] = event.detail;
      fetchSortedTickers(sortCriteria, sortOrder);
  }
</script>

<main>
  <h1>Shares Trading Now</h1>
  <div class="controls-container">
      <StockSort on:sort={handleSort}/>
      <StockSearch on:search={handleSearch}/>
  </div>
  <br/>
  {#if searchedTicker}
      <StockChart ticker={searchedTicker}/>
  {:else if isLoading}
      <h2>Loading Stocks...</h2>
  {:else}
      {#each defaultTickers as ticker}
          <StockChart {ticker}/>
      {:else}
          <h1>No Stocks Found</h1>
      {/each}
  {/if}
</main>

<style>
  h1 {
      text-align: center;
  }

  .controls-container {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 20px;
      margin: 20px 0;
  }
</style>