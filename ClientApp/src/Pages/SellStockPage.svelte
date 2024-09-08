<script>

    import {onMount, afterUpdate, beforeUpdate} from 'svelte';
    import PortalPageMainSpecifiedFirstTicker from "../Components/PortalPageMainSpecifiedFirstTicker.svelte";
    import StockChart from "../Components/StockChartNoButtons.svelte";
    import PortfolioSelect from "../Components/PortfolioSelect.svelte";

    export let ticker;
    let quantity = 1;
    $: quantitySharesHeld = 0;
    let transactionComplete = false;
    $: portfolioId = '';
    let key = 0;    

    $: {
        key += 1;
    }; 

    beforeUpdate(() => {
        portfolioId = '';
    });


    afterUpdate(() => {
        console.log("hi");
        portfolioId = holder;
        if(portfolioId){
            console.log("hello world");
            getQuantitySharesHeld();
        }
    });

    function setPortfolioId(event) {
        portfolioId = event.detail;
        getQuantitySharesHeld();
    }

    async function handleSell() {
        console.log(`Attempting to sell ${quantity} shares`);
        if (portfolioId) {
            try {
                const response = await fetch('/api/StockHoldingApi/SellShares', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        ticker,
                        quantity,
                        portfolioId
                    }),
                });
                
                const data = await response.json();
                
                if (!response.ok) {
                    throw new Error(data.message || 'An error occurred during the transaction');
                }
                
                getQuantitySharesHeld();
                alert("Transaction Successful");
                transactionComplete = true;
                console.log(transactionComplete);
                console.log(data);
            } catch (error) {
                console.error('Error:', error);
                alert(error.message);
            }
        } else {
            alert("Select a Portfolio");
        }
    }

    async function getQuantitySharesHeld(){
        if(portfolioId){
            try{
                const response = await fetch(`/api/StockHoldingApi/GetCurrentQuantitySharesHeld?ticker=${ticker}&portfolioId=${portfolioId}`);
                const data = await response.json();
                quantitySharesHeld = data.quantitySharesHeld;
            } catch (error) {
                console.error('Error', error);
                alert("Problem Fetching Shares Held");
            }
        }
        else {
            quantitySharesHeld = 0;
        }
    }
</script>

<main>
    <h1>Share Sale</h1>
    <div class = "content-wrapper">
        <div class="purchase-section">
            <StockChart {ticker}/>
            {#if portfolioId != ''}
                 <h3>You own {quantitySharesHeld} shares</h3>
            {:else}
                <h3> Select a Portfolio </h3>
            {/if}
            <h2>Sell {ticker} Shares</h2>
            <form on:submit|preventDefault={handleSell}>
                <div class="purchase-inputs">
                    <div class="input-group shares-group">
                        <label for="shares">Shares</label>
                        <input
                            type="number"
                            id="shares"
                            bind:value={quantity}
                            min="1"
                            step="1"
                        >
                    </div>
                    
                    <div class="input-group portfolio-group">
                        <label for="portfolio">Portfolio</label>
                        <PortfolioSelect on:portfolioSelected={setPortfolioId} key = {ticker}/>
                    </div>

                    <div class="input-group submit-group">
                        <button type="submit" class="btn btn-primary">Sell {quantity} Share{ quantity !==1 ?'s':''}</button>
                    </div>
                </div>
            </form>
        </div>
        <div class = "information-section">
            <h3>{ticker} Information</h3>
        </div>
    </div>
    <br/>
    <br/>
    <PortalPageMainSpecifiedFirstTicker on:buttonClicked={portfolioReset} firstTicker={ticker} {key}/>

</main>

<style>

    .content-wrapper {
        display:flex;
        gap:20px;
        margin-bottom: 20px;
    }

    .information-section {
        flex: 1;
        background-color: #f8e6e4;
        border-radius: 8px;
        padding:20px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }

    h1 {
        text-align: center;
    }

    main {
        max-width: 1000px;
        margin: 0 auto;
        padding: 20px;
    }

    .purchase-section {
        /*background-color: #f8f9fa;
        background-color: #B3EBF2;*/
        flex: 0 0 400px;
        background-color: #f8e6e4;
        border-radius: 8px;
        padding: 20px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }

    h2 {
        margin-bottom: 15px;
        color: #333;
        font-size: 20px;
    }

    .purchase-inputs {
        display: flex;
        align-items: flex-end;
        gap: 15px;
    }

    .input-group {
        display: flex;
        flex-direction: column;
    }

    .shares-group {
        flex: 0 1 100px;  /* Allows shrinking but sets a basis of 100px */
    }

    .portfolio-group {
        flex: 1 1 auto;  /* Takes up remaining space */
    }

    .submit-group {
        flex: 0 0 auto;  /* Does not grow or shrink */
        width: 150px;   /*Set a fixed width for the button container */
    }

    label {
        margin-bottom: 5px;
        font-weight: bold;
        color: #555;
        white-space: nowrap;
    }

    input, :global(.portfolio-selector select) {
        width: 100%;
        padding: 8px;
        border: 1px solid #ced4da;
        border-radius: 4px;
        font-size: 14px;
        transition: border-color 0.3s ease;
    }

    input:focus, :global(.portfolio-selector select:focus) {
        outline: none;
        border-color: #007bff;
        box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
    }

    .btn-primary {
        background-color: #007bff;
        color: white;
        border: none;
        padding: 8px 12px;  /* Reduced horizontal padding */
        font-size: 14px;
        border-radius: 4px;
        cursor: pointer;
        transition: background-color 0.3s ease, transform 0.1s ease;
        white-space: nowrap;
        width: 100%;  /* Make the button fill its container */
    }

    .btn-primary:hover {
        background-color: #0056b3;
    }

    .btn-primary:active {
        transform: scale(0.98);
    }

    @media (max-width: 800px) {
        .content-wrapper {
            flex-direction: column;
        }

        .purchase-section, .information-section {
            width: 100%
        }

        .purchase-inputs {
            flex-direction: column;
            align-items: stretch;
        }

        .input-group, .submit-group {
            width: 100%;
        }

        .btn-primary {
            margin-top: 10px;
        }
    }
</style>