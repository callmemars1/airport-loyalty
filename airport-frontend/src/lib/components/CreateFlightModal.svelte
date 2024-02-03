<script lang="ts">
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import {getAirports, getAirlines} from '$lib/scripts/api'
    import {formatDateTime} from "$lib/scripts/utils"
    import {onMount} from 'svelte'

    export let modalId: string

    let airports: [] = []
    let departureAirportData = {}
    $: departureAirportCode = departureAirportData.code
    let arrivalAirportData = {}
    $: arrivalAirportCode = arrivalAirportData.code

    let airlines: [] = []
    let airlineData = {}
    $: airlineCode = airlineData.code

    let departureDate: string = ''
    let departureTime: string = ''

    $: departureDateTimeParsed = Date.parse(`${departureDate} ${departureTime}`)
    $: departureDateTime = new Date(departureDateTimeParsed)

    let errorHappened: boolean = false
    let errors = []

    onMount(async () => {
        airports = await getAirports();
        departureAirportData = airports.find((a) => a.code === 'VKO')
        arrivalAirportData = airports.find((a) => a.code === 'LAX')

        airlines = await getAirlines()
        airlineData = airlines.find((a) => a.code === 'S7')
    })

    const createFlight = async (data) => {
        console.log(data)
        const response = await fetch('/api/flights/create', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        });

        if (response.status === 400) {
            const errorData = await response.json();
            errors = errorData.errors;
            errorHappened = true
            console.log(errors)
        } else if (response.ok) {
            errorHappened = false
            closeModal()
        } else {
            errorHappened = true
        }
    };

    const closeModal = () => document.querySelector(`#${modalId}`).close()

    const handleSubmit = async () => {
        await createFlight({
            departureAirportCode,
            arrivalAirportCode,
            airlineCode,
            departureDateTime
        });
    };

    function addDays(date, days) {
        let result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    let submitPromise: Promise

    $: canSendRequest =
        departureDateTimeParsed
        && airlineCode !== '' && airlineCode !== null && airlineCode !== undefined
        && departureAirportCode !== '' && departureAirportCode !== null && departureAirportCode !== undefined
        && arrivalAirportCode !== '' && arrivalAirportCode !== null && arrivalAirportCode !== undefined
        && arrivalAirportCode !== departureAirportCode
        && departureDateTime > Date.now()


</script>

<dialog id={modalId} class="modal">
    <div class="modal-box w-full">
        <form method="dialog">
            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
        </form>
        <h3 class="font-bold text-lg">Зарегистрировать рейс</h3>
        {#if errorHappened}
            <div class="divider divider-error text-xs">Что-то пошло не так</div>
        {/if}
        <div class="flex flex-col w-full space-y-5">
            <div class="flex flex-col w-full space-y-3">
                <div class="divider divider-neutral">Данные о рейсе</div>
                <div class="flex flex-row gap-2">
                    <div class="flex flex-col w-full">
                        <label for="departure-select" class="block text-sm font-bold mb-2">Аэропорт вылета</label>
                        <select id="departure-select" class="select select-bordered w-full"
                                bind:value={departureAirportData}>
                            {#each airports as airportOption}
                                <option value={airportOption}>
                                    <span class="font-mono text-primary/75">[{airportOption.code}]</span>
                                    {airportOption.city}
                                </option>
                            {/each}
                        </select>
                        <ErrorsList bind:errors={errors.departureAirportCode}/>
                    </div>
                    <div class="flex flex-col w-full">
                        <label for="arrival-select" class="block text-sm font-bold mb-2">Аэропорт прилета</label>
                        <select id="arrival-select" class="select select-bordered w-full"
                                bind:value={arrivalAirportData}>
                            {#each airports as airportOption}
                                <option value={airportOption}>
                                    <span class="font-mono text-primary/75">[{airportOption.code}]</span>
                                    {airportOption.city}
                                </option>
                            {/each}
                        </select>
                        <ErrorsList bind:errors={errors.arrivalAirportCode}/>
                    </div>
                </div>
                <div class="flex flex-col w-full">
                    <label for="departure-select" class="block text-sm font-bold mb-2">Авиакомпания</label>
                    <select id="departure-select" class="select select-bordered w-full"
                            bind:value={airlineData}>
                        {#each airlines as airlineOption}
                            <option value={airlineOption}>
                                <span class="font-mono text-primary/75">[{airlineOption.code}]</span>
                                {airlineOption.title}
                            </option>
                        {/each}
                    </select>
                    <ErrorsList bind:errors={errors.airlineCode}/>
                    <div class="flex flex-col t-2">
                        <div class="divider m-0 p-0"></div>
                        <p class="text-gray-700 text-xs">Самолет будет подобран авиакомпанией.</p>
                    </div>
                </div>
                <div class="form-control w-full">
                    <div class="flex flex-row justify-between">
                        <label class="block text-sm font-bold mb-2">Дата и время вылета</label>
                        <label class="block text-xs font-mono mb-2">{formatDateTime(departureDateTime)}</label>
                    </div>
                    <div class="flex flex-row gap-4">
                        <input type="date" class="input input-bordered w-full" bind:value={departureDate}/>
                        <input type="time" class="input input-bordered w-full" bind:value={departureTime}/>
                    </div>
                    <ErrorsList bind:errors={errors.departureDateTime}/>
                </div>
            </div>
            
            <div class="flex flex-row justify-between">
                <button class="btn btn-ghost" on:click={closeModal}>Назад</button>
                <button class="btn btn-primary btn-outline" type="submit" on:click={handleSubmit}
                        disabled={!canSendRequest}>
                    Продолжить
                </button>
            </div>
        </div>
    </div>
</dialog>