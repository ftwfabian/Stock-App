<script>
    import { createEventDispatcher } from "svelte";

    //to be implemented later. Not all that worried in implementing it right now
    //let selectedSort = fetch('/api/UserApi/GetPortalSortPreference')
    let selectedSort = "marketcap,desc";
    const dispatch = createEventDispatcher();

    async function handleSort(){

        const[sortCriteria, sortOrder] = selectedSort.split(',');
        
        const url = new URL('/api/StockApi/GetSortedTickers',window.location.origin);
        url.searchParams.append('sortCriteria',sortCriteria);
        url.searchParams.append('sortOrder',sortOrder);

        const response = await fetch(url);
        if(response.ok) {
            const data = await response.json();
            dispatch('sort',data);
        } else {
            console.error('Error fetching sorted tickers');
        }
    }

    const sortOptions = [
        {value: "favorite,na", label: "Favorites"},
        {value: "marketcap,desc", label: "Market Cap High to Low"},
        {value: "marketcap,asc", label: "Market Cap Low to High"},
    ];
</script>

<main>
    <div class = "sort-controls">
        <label for = "sortSelect">Sort by</label>
        <select id = "sortSelect" bind:value={selectedSort} on:change={handleSort}>
            {#each sortOptions as option}
                <option value = {option.value}>{option.label}</option>
            {/each}
        </select>
    </div>

</main>

<style>
    .sort-controls {
        display: flex;
        align-items: center;
        gap: 10px;
    }
    select {
        /*padding: 5px;*/
        height: 38px;
    }
    label{
        /*font-weight:bold;*/
        font-size: large;

    }
    
</style>