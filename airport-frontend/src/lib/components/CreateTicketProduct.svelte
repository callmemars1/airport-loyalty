<script lang="ts">
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'
    import {getAirports, getAirlines} from '$lib/scripts/api'
    import {formatDateTime} from "$lib/scripts/utils"
    import {getFlightByNumber, getServiceClassesForFlight} from "$lib/scripts/api"
    import {onMount} from 'svelte'

    export let modalId: string

    let flightNumber: string = ""
    let flight: {}
    let serviceClasses = []
    let serviceClassesInputs = []

    $: fetchFlight(flightNumber)
    
    $: rowClassProducts = serviceClassesInputs
        .filter((s) => s.include)
        .map((s) => {
        return {
            rowClassId: s.serviceClass.id,
            price: s.price
        }
    })
    
    $: flightId = flight === undefined ? '' : flight.id
    
    $: console.log(rowClassProducts)


    let errorHappened: boolean = false
    let errors = []

    onMount(async () => {
    })

    const createProduct = async (data) => {
        console.log(data)
        const response = await fetch('/api/flights/create-product', {
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

    const fetchFlight = async () => {
        if (flightNumber.length !== 8)
            return

        let result = await getFlightByNumber(flightNumber)
        if (result.length <= 0)
            return

        flight = result[0]
        serviceClasses = await getServiceClassesForFlight(flight.id)
        serviceClassesInputs = serviceClasses.map((sc) => {
            return {
                serviceClass: sc,
                include: false,
                price: 0
            }
        })
    }

    const closeModal = () => document.querySelector(`#${modalId}`).close()

    const handleSubmit = async () => {
        await createProduct({
            rowClassProducts,
            flightId
        });
    };

    function addDays(date, days) {
        let result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    let submitPromise: Promise
    
    $: canSendRequest = serviceClassesInputs.find((s) => s.include)

    /*    $: canSendRequest =
            departureDateTimeParsed
            && airlineCode !== '' && airlineCode !== null && airlineCode !== undefined
            && departureAirportCode !== '' && departureAirportCode !== null && departureAirportCode !== undefined
            && arrivalAirportCode !== '' && arrivalAirportCode !== null && arrivalAirportCode !== undefined
            && arrivalAirportCode !== departureAirportCode
            && departureDateTime > Date.now()*/


</script>

<dialog id={modalId} class="modal">
    <div class="modal-box w-full">
        <form method="dialog">
            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
        </form>
        <h3 class="font-bold text-lg">Создать билет</h3>

        <div class="divider divider-neutral">Выбор рейса</div>

        <LabeledFormInput
                title="Номер рейса"
                id="flightNumber"
                placeholder="AA012345"
                inputType="text"
                bind:value={flightNumber}
                errors={errors.flightNumber}
                required
        />

        {#if flight}
            <div class="divider divider-primary">
                [{flight.departureAirport.code}] {flight.departureAirport.city} ---> [{flight.arrivalAirport.code}
                ] {flight.arrivalAirport.city}
            </div>

            <div class="grid overflow-hidden">
                <div class="overflow-x-auto space-y-5 flex flex-col justify-center">
                    <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2">
                        <thead>
                        <tr class="bg-primary text-white">
                            <th>Класс</th>
                            <th>№ 1-го ряда</th>
                            <th>Мест</th>
                            <th>Стоимость</th>
                            <th></th>
                        </tr>
                        </thead>
                        <tbody>
                        {#each serviceClassesInputs as serviceClassInput}
                            <tr>
                                <td>{serviceClassInput.serviceClass.title}</td>
                                <td>
                                    {serviceClassInput.serviceClass.rowsOffset}
                                </td>
                                <td>
                                    {serviceClassInput.serviceClass.rowsCount * serviceClassInput.serviceClass.seatsPerRow}
                                </td>
                                <td>
                                    <div class="flex flex-row content-center space-x-3">
                                        <span class="text-bold">₽</span>
                                        <input type="number" 
                                               class="input input-bordered w-full h-6 p-1 text-sm"
                                               bind:value={serviceClassInput.price}
                                               disabled={!serviceClassInput.include}
                                               min="0"
                                               max="1000000"
                                        >
                                    </div>
                                </td>
                                <td>
                                    <input type="checkbox"
                                           bind:checked={serviceClassInput.include} 
                                           class="checkbox checkbox-primary"/>
                                </td>
                            </tr>
                        {/each}
                        </tbody>
                    </table>
                </div>
            </div>
        {/if}

        <br>

        <div class="flex flex-row justify-between">
            <button class="btn btn-ghost" on:click={closeModal}>Назад</button>
            <button class="btn btn-primary btn-outline" type="submit" on:click={handleSubmit} disabled={!canSendRequest}>
                Продолжить
            </button>
        </div>

    </div>
</dialog>