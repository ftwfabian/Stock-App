<script>
  import { onMount } from 'svelte';
  import StockChart from "../Components/StockChartBuySell.svelte";
  import StockSearch from '../Components/StockSearch.svelte';
  import StockSort from '../Components/StockSort.svelte';

  $: defaultTickers = [];
  let searchedTicker = '';
  let isLoading = false;

  async function fetchSortedTickers(sortCriteria, sortOrder){
    isLoading = true;
    try{
      const url = new URL('/api/StockApi/GetSortedTickers',window.location.origin);
      url.searchParams.append('sortCriteria',sortCriteria);
      url.searchParams.append('sortOrder',sortOrder);
      const response = await fetch(url);

      if(!response.ok){
        throw new Error("Network Response was not Ok");
      }
      const newTickers = await response.json()
      defaultTickers = [...newTickers];
    } catch(error){
      console.error('There was a problem fetching the tickers',error);
    } finally {
      isLoading = false;
    }
  }

  onMount(async () => {
    const [sortCriteria,sortOrder] = ['marketcap','desc'];
    fetchSortedTickers(sortCriteria,sortOrder);
  });


  function handleSearch(event){
    //obviously, for finding related tickers, that will happen in the event being presented
    //from StockSearch
    searchedTicker = event.detail;
  }

  function handleSort(event){
    defaultTickers = event.detail;
    fetchSortedTickers(sortCriteria,sortOrder);
  }
</script>

<main>
  <h1>Shares Trading Now</h1>
  <div class = "controls-container">
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
  h1{
    text-align:center;
  }

  .controls-container{
    display:flex;
    justify-content: center;
    align-items: center;
    gap: 20px;
    margin: 20px 0;
  }
</style>