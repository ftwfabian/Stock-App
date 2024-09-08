<script>

  import { onMount } from "svelte";

  let isFavorite;
  export let ticker;

  async function fetchFavoriteStatus(){
    try{
      const response = await fetch(`/api/UserApi/GetFavoriteStockBoolean?ticker=${encodeURIComponent(ticker)}`);
      const result = await response.json();
      if(!response.ok) throw new Error(result.message || `HTTP error! status: ${response.status}`);
      isFavorite = result.tickerIsFavorite;
    } catch {
      console.error('Error fetching favorite status: ',error);
    }
  }

  onMount(fetchFavoriteStatus);
  
  async function toggleFavorite() {
    try {
      if(!isFavorite){
        const response = await fetch(`/api/UserApi/AddFavoriteStock?ticker=${encodeURIComponent(ticker)}`, {
          method: 'POST'});
        
        const result = await response.json();
        if (!response.ok) throw new Error(result.message || `HTTP error! status: ${response.status}`);
        
        console.log(result.message);
        
      } else {
        const response = await fetch(`/api/UserApi/DeleteFavoriteStock?ticker=${encodeURIComponent(ticker)}`, {
          method:'DELETE'});
        const result = await response.json();
        if(!response.ok) throw new Error(result.message || `HTTP error! status: ${response.status}`);

        console.log(result.message);
      }

      isFavorite = !isFavorite;
    
  } catch (error) {
    alert(error.message);
    console.error('There was a problem toggling Favorite Stock');
  }
    // Here you would typically call an API to update the favorite status
    // For now, we'll just toggle the local state
  }
</script>
  
  <button on:click={toggleFavorite} class="favorite-star" aria-label="Toggle favorite">
    {#if isFavorite}
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="yellow" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
      </svg>
    {:else}
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
      </svg>
    {/if}
  </button>
  
  <style>
    .favorite-star {
      background: none;
      border: none;
      cursor: pointer;
      padding: 0;
    }
    .favorite-star:hover svg {
      stroke: #ffd700;
    }
  </style>