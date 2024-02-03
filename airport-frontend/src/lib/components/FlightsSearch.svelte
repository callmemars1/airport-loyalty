<script lang="ts">
    import {onMount} from "svelte";
    import {formatDateTime} from "$lib/scripts/utils"
    import {getFlightsBySearchParameters, getFlightByNumber, getAirports, PageSize} from "$lib/scripts/api"

    let flights = [];
    export let flightsLoading = true;

    let flightNumber: string = ''

    $: searchByFlightNumber = flightNumber.length > 0

    let pageNumber = 1;
    let departureAirportCode = '';
    let arrivalAirportCode = '';
    let departureDateMin = '';
    let departureDateMax = '';

    $: searchByParameters =
        departureAirportCode.length > 0
        ||  arrivalAirportCode.length > 0
        ||  departureDateMin.length > 0
        ||  departureDateMax.length > 0

    $: cantSearchForFlightByNumber =
        searchByFlightNumber && (!flightNumber || flightNumber.length === 0)

    $: cantSearchForFlightByParams =
        searchByParameters && (
            !departureAirportCode || departureAirportCode.length === 0
            ||  !arrivalAirportCode || arrivalAirportCode.length === 0
            ||  arrivalAirportCode === departureAirportCode
            ||  (departureDateMin && departureDateMax && departureDateMin >= departureDateMax))

    $: cantSearch =
        (!searchByParameters && !searchByFlightNumber)
        || cantSearchForFlightByParams
        || cantSearchForFlightByNumber


    const clearAllFields = () => {
        departureAirportCode = '';
        arrivalAirportCode = '';
        departureDateMin = '';
        departureDateMax = '';
        flightNumber = ''
    }


    let airports = [];
    export let airportsLoading = true;

    async function fetchFlights() {
        flightsLoading = true;
        if (searchByParameters)
            flights = await getFlightsBySearchParameters(departureAirportCode, arrivalAirportCode, pageNumber, departureDateMin)
        else
            flights = await getFlightByNumber(flightNumber)

        console.log(flightNumber)

        flightsLoading = false;
    }

    async function fetchAirports() {
        airportsLoading = true;
        airports = await getAirports();
        airportsLoading = false;
    }

    onMount(async () => {
        await fetchAirports()
    });

</script>

<div class="flex flex-col space-y-5 pb-5">

    <div class="flex flex-row w-full">
        <label class="form-control w-full">
            <div class="label">
                Номер рейса
            </div>
            <input
                    type="text" class="input input-bordered w-full" bind:value={flightNumber}
                    disabled={searchByParameters}
            >
        </label>
    </div>

    <div class="divider">или</div>

    <div class="flex flex-row space-x-4 mb-4 justify-between">

        <label class="form-control w-full max-w-xs">
            <div class="label">
                <span class="label-text">Откуда</span>
            </div>
            <select
                    class="select select-bordered" bind:value={departureAirportCode}
                    disabled={searchByFlightNumber}
            >
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
            <select
                    class="select select-bordered w-full" bind:value={arrivalAirportCode}
                    disabled={searchByFlightNumber}
            >
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
                <span class="label-text">От</span>
            </label>
            <input
                    type="date" class="input input-bordered w-full" bind:value={departureDateMin}
                    disabled={searchByFlightNumber}
            />
        </div>


        <div class="form-control w-full max-w-xs">
            <label class="label">
                <span class="label-text">До</span>
            </label>
            <input
                    type="date" class="input input-bordered w-full" bind:value={departureDateMax}
                    disabled={searchByFlightNumber}
            />
        </div>

    </div>

    <div class="flex flex-row w-full space-x-4">
        <button class="btn btn-accent flex-1" disabled={cantSearch} on:click={async () => {
                pageNumber = 1
                await fetchFlights()
            }}>
            Поиск
        </button>
        <button class="btn btn-outline btn-warning flex-none" on:click={clearAllFields}
        >
            Очистить
        </button>
    </div>

    <br>

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
                        disabled={flights.length - PageSize < 0}
                >
                    »
                </button>
            </div>
        </div>
    </div>
</div>