<script lang="ts">
    import Clock from '$lib/components/Clock.svelte';
    import DepartingArrivingFlightsTable from '$lib/components/DepartingArrivingFlightsTable.svelte';
    import FlightsSearch from '$lib/components/FlightsSearch.svelte';

    let flightsLoading = true;
    let searchLoading = true;
</script>

<main class="px-20 space-y-10">
    <div class="flex flex-row justify-between">
        <h1 class="font-bold text-xl">Онлайн-табло</h1>
        <Clock/>
    </div>

    <div class="divider divider-primary">
        Ближайшие рейсы
        {#if flightsLoading}
            <span class="loading loading-ring loading-lg"></span>
        {/if}
    </div>


    <DepartingArrivingFlightsTable bind:loading={flightsLoading} />


    <div class="divider divider-primary">
        Поиск рейсов
        {#if flightsLoading}
            <span class="loading loading-ring loading-lg"></span>
        {/if}
    </div>
    
    <FlightsSearch bind:flightsLoading={searchLoading}/>
</main>