<script>

import { onMount, afterUpdate } from "svelte";
import { createEventDispatcher } from "svelte";

const dispatch = createEventDispatcher();

export let key;

let userPortfolios = [];
let selectedPortfolio = null;
let previousKey = key;

onMount(async () => {
    await getUserPortfolios();
});

afterUpdate(() => {
    if (key !== previousKey) {
        selectedPortfolio = null;
        dispatch('portfolioSelected', null);
        previousKey = key;
    }
});

async function getUserPortfolios()
{
    const response = await fetch(`/api/UserApi/GetUserPortfolios`);
    const data = await response.json();
    userPortfolios = data.result;
    console.log(userPortfolios);
}

function handlePortfolioSelection(){
    dispatch('portfolioSelected', selectedPortfolio);
}

</script>

<main>

<div class = "portfolio-selector">
    <select id="portfolioSelect" bind:value={selectedPortfolio} on:change={handlePortfolioSelection}>
        <option value = {null}>...</option>
        {#each userPortfolios as portfolio (portfolio.id)}
            <option value={portfolio.id}>{portfolio.name}</option>
        {/each}
    </select>
</div>

</main>

<style>
    select {
        height: 40px;
        width: 130px;
    }
</style>