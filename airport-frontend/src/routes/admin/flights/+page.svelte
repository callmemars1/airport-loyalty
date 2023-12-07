<script lang="ts">
    import {onMount} from "svelte";
    import FlightSearch from "$lib/components/FlightsSearch.svelte"

    let flights = [];
    export let flightsLoading = true;
    export let airportsLoading = true;
    let pageNumber = 1;
    const pageSize = 10;
    let departureAirportCode = '';
    let arrivalAirportCode = '';
    let departureDate = '';

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
</script>


<div class="flex flex-col space-y-10">
    <div class="flex flex-col space-y-5">
        <h1 class="text-2xl font-bold">Рейсы</h1>
        <p>Тут вы можете просматривать информацию о рейсах, а также редактировать их.</p>
    </div>

    <div class="divider">Список рейсов</div>

    <FlightSearch />
</div>