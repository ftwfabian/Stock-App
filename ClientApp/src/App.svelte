<script>
  import { Router, Route, Link } from "svelte-routing";
  import PortalPageMain from "./Pages/PortalPageMain.svelte";
  import BuyStockPage from "./Pages/BuyStockPage.svelte";
  import SellStockPage from "./Pages/SellStockPage.svelte";

  export let url = "";
</script>

<Router {url}>
  <nav>
    <Link to="/">Home</Link>
    <Link to="/Portal">Portal</Link>
  </nav>

  <main>
    <Route path="/Portal">
      <PortalPageMain/>
    </Route>
    <Route path="/Portal/buy/:ticker" let:params>
      <BuyStockPage ticker={params.ticker}/>
    </Route>
    <Route path="/Portal/sell/:ticker" let:params>
      <SellStockPage ticker={params.ticker}/>
    </Route>
    <Route path="/">
      <h1>Welcome to your Stock App</h1>
    </Route>
  </main>
</Router>

<style>
  main {
    text-align: center;
    padding: 1em;
    margin: 0 auto;
  }
</style>