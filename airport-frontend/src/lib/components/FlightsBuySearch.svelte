<script lang="ts">
    import {onMount} from 'svelte';
    import CartLogo from '$lib/components/CartLogo.svelte';
    import EditLogo from '$lib/components/EditLogo.svelte';
    import EditTicketProductModal from '$lib/components/EditTicketProductModal.svelte';


    let products = [];
    export let flightsLoading = true;
    export let airportsLoading = true;
    export let adminPage = false;

    let pageNumber = 1;
    const pageSize = 10;
    let departureAirportCode = '';
    let arrivalAirportCode = '';
    let departureDate = ''

    let showAllTickets = false

    // Dummy airports array - replace this with your actual API call later
    let airports = [];

    async function fetchFlights() {
        flightsLoading = true;
        let response: {}
        try {
            if (showAllTickets) {
                response = await fetch(`/api/products?pageNumber=${pageNumber}&pageSize=${pageSize}`)
            } else {
                response = await fetch(
                    `/api/products/tickets?departureAirportCode=${departureAirportCode}
                &arrivalAirportCode=${arrivalAirportCode}
                ${departureDate ? '&departureDate=' + departureDate : ''}
                &pageNumber=${pageNumber}
                &pageSize=${pageSize}
                ${price ? '$maxPrice=' + price : ''}`
                );
            }

            const data = await response.json();
            products = data;
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

    let buyTicketErrors = {}
    let buyTicketSuccess = {}

    const buyTicket = async (ticketId: string) => {
        try {
            let result = await fetch(`/api/products/buy-ticket?productId=${ticketId}&quantity=1`, {
                    method: 'POST',
                }
            );
            const data = await result.json();
            if (result.ok) {
                buyTicketSuccess[ticketId] = products.find(p => p.id === ticketId)
                setTimeout(() => {
                    delete buyTicketSuccess[ticketId]
                    buyTicketSuccess = buyTicketSuccess
                }, 3000)
                await fetchFlights()
            }
            if (result.status === 400) {
                buyTicketErrors[ticketId] = data.errors
                setTimeout(() => {
                    delete buyTicketErrors[ticketId]
                    buyTicketErrors = buyTicketErrors
                }, 3000)
            }
        } catch (error) {
            console.error('Failed to buy ticket', error);
            throw error;
        }
    }

    let formatTwoDigit = (num: number) => {
        if (num % 10 === num)
            return `0${num}`
        return num.toString()
    }

    let formatDateTime = (date: Date) => {
        return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
    }

    // ---------
    let price: number;
</script>

<div class="flex flex-col space-y-5 pb-5">

    <div class="flex flex-row w-full">
        <div class="flex flex-col justify-center">
            <div class="form-control">
                <label class="cursor-pointer label space flex flex-col content-center text-center">
                    <span class="label label-text">Показать все билеты</span>
                    <input type="checkbox" bind:checked={showAllTickets} class="checkbox checkbox-accent"/>
                </label>
            </div>
        </div>
        <div class="divider divider-horizontal divider-primary">или</div>

        <div class="flex flex-col space-x-4 mb-4 justify-between content-center w-full">

            <div class="flex flex-row space-x-4 mb-4 justify-between content-center">
                <label class="form-control w-full max-w-xs">
                    <div class="label">
                        <span class="label-text">Откуда</span>
                    </div>
                    <select class="select select-bordered" bind:value={departureAirportCode} disabled={showAllTickets}>
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
                    <select class="select select-bordered w-full" bind:value={arrivalAirportCode}
                            disabled={showAllTickets}>
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
                    <input type="date" class="input input-bordered w-full" bind:value={departureDate}
                           placeholder="Не выбрано" disabled={showAllTickets}/>
                </div>

                <div class="form-control w-full max-w-xs">
                    <label class="label">
                        <span class="label-text">Максимальная стоимость</span>
                    </label>
                    <input type="number"
                           class="input input-bordered w-full"
                           bind:value={price}
                           placeholder="Не выбрано"
                           min="0"
                           max="1000000" disabled={showAllTickets}/>
                </div>

            </div>
            <div class="form-control w-full h-min self-end">
                <button class="btn btn-primary" on:click={fetchFlights}
                        disabled={!showAllTickets && (!departureAirportCode || !arrivalAirportCode || arrivalAirportCode === departureAirportCode)}
                >
                    Поиск
                </button>
            </div>
        </div>
    </div>
    <div class="grid overflow-hidden">

        <div class="overflow-x-auto space-y-5 flex flex-col justify-center">
            <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2">
                <thead>
                <tr class="bg-primary text-white">
                    <th>Название</th>
                    <th>Авиакомпания</th>
                    <th>Время вылета</th>
                    <th>Время прибытия</th>
                    <th>Мест осталось</th>
                    <th>Цена</th>
                    {#if adminPage}
                        <th>Редактировать</th>
                    {:else }
                        <th>Купить</th>
                    {/if}
                </tr>
                </thead>
                <tbody>
                {#each products as product (product.id)}
                    {@const departureDate = new Date(product.flight.departureDateTime)}
                    {@const arrivalDate = new Date(product.flight.arrivalDateTime)}
                    {@const placesCount = product.serviceClass.rowsCount * product.serviceClass.seatsPerRow}
                    {@const emptyPlaces = placesCount - product.boughtTicketsCount}
                    {@const flightAlreadyDeparted = departureDate < Date.now()}
                    {@const modalId = `update_ticket_modal_${product.id}`}
                    <tr class={flightAlreadyDeparted ? 'text-red-700': ''}>
                        <td>{product.title}</td>
                        <td>
                            {product.flight.airline.title}
                        </td>
                        <td>
                            <div class="flex flex-col space-y-2">
                                {formatDateTime(departureDate)}
                                {#if flightAlreadyDeparted}
                                    <span class="text-xs text-red-700">Улетел</span>
                                {/if}
                            </div>
                        </td>
                        <td>
                            {formatDateTime(arrivalDate)}
                        </td>
                        <td><span
                                class={flightAlreadyDeparted ? 'text-red-700': 'text-primary'}>{emptyPlaces}</span>/{placesCount}
                        </td>
                        <td>₽ {product.price}</td>
                        {#if adminPage}
                            <td>
                                <button

                                        class="btn btn-primary"
                                        disabled={flightAlreadyDeparted}
                                        on:click={document.getElementById(modalId).showModal()}
                                >
                                    <EditLogo/>
                                </button>
                                {#if product}
                                    <EditTicketProductModal modalId={modalId} ticket={product}
                                                            onEditEnded={fetchFlights}/>
                                {/if}
                            </td>
                        {:else }
                            <td>
                                <button
                                        class={buyTicketErrors[product.id] ? 'btn btn-error' : 'btn btn-primary' }
                                        on:click={() => buyTicket(product.id)}
                                        disabled={flightAlreadyDeparted}>
                                    <CartLogo/>
                                </button>
                            </td>
                        {/if}
                    </tr>
                {/each}
                {#if products.length === 0}
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
                        disabled={products.length - pageSize < 0}
                >
                    »
                </button>
            </div>
        </div>
    </div>
</div>

{#if Object.entries(buyTicketErrors).length !== 0}
    {#each Object.entries(buyTicketErrors) as [key, value] (key)}
        <div role="alert" class="alert alert-error relative z-50" >
            <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6"
                 fill="none" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <span>Ошибка! {value[0]}</span>
        </div>
    {/each}
{/if}


{#if Object.entries(buyTicketSuccess).length !== 0}
    {#each Object.entries(buyTicketSuccess) as [key, value] (key)}
        <div role="alert" class="alert alert-success">
            <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
            <span>Поздравляем! Вы оформили билет!</span>
        </div>
    {/each}
{/if}