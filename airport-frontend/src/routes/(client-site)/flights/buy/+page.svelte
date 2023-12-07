<script lang="ts">
    import Authorized from '$lib/components/Authorized.svelte'
    import FlightsBuySearch from '$lib/components/FlightsBuySearch.svelte'
    
    let flightsLoading: boolean
</script>

<Authorized />

<main class="px-20">
<div class="divider divider-primary">
    Купить билеты
    {#if flightsLoading}
        <span class="loading loading-ring loading-lg"></span>
    {/if}
</div>

<FlightsBuySearch />
    
    
</main>