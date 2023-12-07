<script lang="ts">
    import {onMount} from 'svelte';
    import CartLogo from '$lib/components/CartLogo.svelte';

    let flights = [];
    export let flightsLoading = true;
    export let airportsLoading = true;
    let pageNumber = 1;
    const pageSize = 10;
    let departureAirportCode = '';
    let arrivalAirportCode = '';
    let departureDate = ''

    // Dummy airports array - replace this with your actual API call later
    let airports = [];

    async function fetchFlights() {
        flightsLoading = true;
        try {
            const response = await fetch(
                `/api/flights/filtered?departureAirportCode=${departureAirportCode}&arrivalAirportCode=${arrivalAirportCode}${departureDate ? '&departureDate=' + departureDate : ''}&pageNumber=${pageNumber}&pageSize=${pageSize}`
            );
            const data = await response.json();
            flights = data;
            // Update totalFlights if your API provides it
        } catch (error) {
            console.error('Failed to fetch flights:', error);
        }
        flightsLoading = false;
    }

    async function fetchAirports() {
        airportsLoading = true;
        try {
            const response = await fetch(
                `/api/flights/airports`
            );
            const data = await response.json();
            airports = data;
        } catch (error) {
            console.error('Failed to fetch airports:', error);
        }
        airportsLoading = false;
    }

    onMount(async () => {
        await fetchAirports()
    });

    let formatTwoDigit = (num: number) => {
        if (num % 10 === num)
            return `0${num}`
        return num.toString()
    }

    let formatDateTime = (date: Date) => {
        return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
    }

    // ---------
    
    let maxPrice = 10500;
    let minPrice = 1999;
    let price = minPrice;
</script>

<div class="flex flex-col space-y-5 pb-5">
    <div class="flex flex-row space-x-4 mb-4 justify-between">
        <label class="form-control w-full max-w-xs">
            <div class="label">
                <span class="label-text">Откуда</span>
            </div>
            <select class="select select-bordered" bind:value={departureAirportCode}>
                {#each airports as airport}
                    <option value={airport.code}>
                        <span class="font-mono text-primary/75">[{airport.code}]</span>
                        {airport.city}
                    </option>
                {/each}
            </select>
        </label>

        <div class="form-control w-full max-w-xs">
            <div class="label">
                <span class="label-text">Куда</span>
            </div>
            <select class="select select-bordered w-full" bind:value={arrivalAirportCode}>
                {#each airports as airport}
                    <option value={airport.code} class="font-mono">
                        [{airport.code}]
                        {airport.city}
                    </option>
                {/each}
            </select>
        </div>

        <div class="form-control w-full max-w-xs">
            <label class="label">
                <span class="label-text">Когда</span>
            </label>
            <input type="date" class="input input-bordered w-full" bind:value={departureDate} placeholder="Не выбрано"/>
        </div>

        <div class="form-control w-full max-w-xs">
            <label class="label">
                <span class="label-text">Цена</span>
            </label>
            <div class="flex flex-col h-full justify-center">
                <input type="range" min="{minPrice}" max="{maxPrice}" class="range range-primary" bind:value={price}/>
                <div class="flex flex-row flex-none justify-between text-gray-500 text-xs">
                    <span>{minPrice}</span>
                    <span class="text-primary">{price}</span>
                    <span>{maxPrice}</span>
                </div>
            </div>
        </div>

        <div class="form-control w-full max-w-xs h-min self-end">
            <button class="btn btn-primary" on:click={fetchFlights}
                    disabled={!departureAirportCode || !arrivalAirportCode || arrivalAirportCode === departureAirportCode}
            >
                Поиск
            </button>
        </div>
    </div>
    <div class="grid overflow-hidden">

        <div class="overflow-x-auto space-y-5 flex flex-col justify-center">
            <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2">
                <thead>
                <tr class="bg-primary text-white">
                    <th>Рейс</th>
                    <th>Откуда</th>
                    <th>Куда</th>
                    <th>Авиакомпания</th>
                    <th>Время вылета</th>
                    <th>Мест осталось</th>
                    <th>Цена</th>
                    <th>Купить</th>
                </tr>
                </thead>
                <tbody>
                {#each flights as flight}
                    {@const date = new Date(flight.departureDateTime)}
                    <tr>
                        <td>{flight.flightNumber}</td>
                        <td>
                            <span class="font-mono text-primary/75">[{flight.departureAirport.code}]</span>
                            {flight.departureAirport.city}
                        </td>
                        <td>
                            <span class="font-mono text-primary/75">[{flight.arrivalAirport.code}]</span>
                            {flight.arrivalAirport.city}
                        </td>
                        <td>{flight.airline.title}</td>
                        <td>{formatDateTime(date)}</td>
                        <td><span class="text-primary">10</span>/200</td>
                        <td>228 р.</td>
                        <td>
                            <button class="btn btn-primary">
                                <CartLogo/>
                            </button>
                        </td>
                    </tr>
                {/each}
                {#if flights.length === 0}
                    <span class="p-4">Ничего не найдено</span>
                {/if}
                </tbody>
            </table>

            <div class="join self-center">
                <button
                        class="join-item btn"
                        on:click={() => {
                            pageNumber = pageNumber - 1
                            fetchFlights()
                        }}
                        disabled={pageNumber <= 1}
                >
                    «
                </button>
                <button class="join-item btn">{pageNumber}</button>
                <button
                        class="join-item btn"
                        on:click={() => {
                            pageNumber = pageNumber + 1
                            fetchFlights()
                        }}
                        disabled={flights.length - pageSize < 0}
                >
                    »
                </button>
            </div>
        </div>
    </div>
</div>