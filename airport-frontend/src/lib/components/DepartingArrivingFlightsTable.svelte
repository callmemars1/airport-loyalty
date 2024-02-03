<script lang="ts">
    import ToggledView from '$lib/components/ToggledView.svelte';
    import {onMount, onDestroy} from 'svelte';
    import {getDepartingFlights, getArrivingFlights} from '$lib/scripts/api'
    import type {TableFlightDto} from '$lib/scripts/dto'
    
    export let loading; // Preloader state

    let departingFlights: TableFlightDto[] = [];
    let arrivingFlights: TableFlightDto[] = [];

    async function fetchFlights() {
        try {
            loading = true; // Show preloader
            departingFlights = await getDepartingFlights("VKO", new Date().toISOString());
            arrivingFlights = await getArrivingFlights("VKO", new Date().toISOString());
        } catch (error) {
            console.error('Error fetching flights:', error);
            // Handle error appropriately
        } finally {
            loading = false; // Hide preloader
        }
    }
    
    let interval

    onMount(async () => {
        if (interval)
            return
        
        await fetchFlights(); // Initial fetch

        // Set interval to fetch flights every 5 seconds
        interval = setInterval(fetchFlights, 5000);
    });

    // Clear interval on component destruction
    onDestroy(() => {
        clearInterval(interval);
    });
    
    let formatTwoDigit = (num: number) => {
        if (num % 10 === num)
            return `0${num}`
        return num.toString()
    }

    let formatDateTime = (date: Date) => {
        return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
    }

    let departuresShown: boolean = true
</script>


<div class="flex flex-row justify-center">
    <ToggledView bind:isToggled={departuresShown}/>
</div>

<div class="grid overflow-hidden">

    {#if departuresShown}

        <div class="overflow-x-auto space-y-5">
            <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2"
            >
                <thead>
                <tr class="bg-primary text-white">
                    <th>Рейс</th>
                    <th>Направление</th>
                    <th>Выход</th>
                    <th>Авиакомпания</th>
                    <th>Время вылета</th>
                </tr>
                </thead>
                <tbody>
                {#each departingFlights as flight}
                    {@const date = new Date(flight.departureDateTime)}
                    <tr>
                        <td>{flight.flightNumber}</td>
                        <td>
                            <span class="font-mono text-primary/75">[{flight.arrivalAirport.code}]</span>
                            {flight.arrivalAirport.city}</td>
                        <td>{flight.departureGate.terminal}-{flight.departureGate.name}</td>
                        <td>{flight.airline.title}</td>
                        <td>{formatDateTime(date)}</td>
                    </tr>
                {/each}
                </tbody>
            </table>
        </div>

    {:else }

        <!-- Arrivals Table -->
        <div class="overflow-x-auto space-y-5">
            <table class="table table-zebra w-full border border-accent col-start-1 col-end-2 row-start-1 row-end-2"
            >
                <thead>
                <tr class="bg-accent text-white">
                    <th>Рейс</th>
                    <th>Город вылета</th>
                    <th>Выход</th>
                    <th>Авиакомпания</th>
                    <th>Время прилета</th>
                </tr>
                </thead>
                <tbody>
                {#each arrivingFlights as flight}
                    {@const date = new Date(flight.arrivalDateTime)}
                    <tr>
                        <td>{flight.flightNumber}</td>
                        <td>
                            <span class="font-mono text-primary/75">[{flight.departureAirport.code}]</span>
                            {flight.departureAirport.city}</td>
                        <td>{flight.departureGate.terminal}-{flight.departureGate.name}</td>
                        <td>{flight.airline.title}</td>
                        <td>{formatDateTime(date)}</td>
                    </tr>
                {/each}
                </tbody>
            </table>
        </div>

    {/if}
</div>