<script lang="ts">
    import {onMount} from 'svelte'
    import UpdateUserModal from '$lib/components/UpdateUserModal.svelte'
    import {getUserData} from '$lib/scripts/api'
    import {formatDateTime} from '$lib/scripts/utils'
    import {error} from '@sveltejs/kit';
    import {page} from '$app/stores';

    let promise = getUserData()
    
    let tickets = []

    async function fetchTickets() {
        try {
            const response = await fetch(
                `/api/products/get-tickets`
            );
            const data = await response.json();
            tickets = data;
        } catch (error) {
            console.error('Failed to fetch tickets:', error);
        }
    }

    onMount(async () => {
        let user = await promise
        if ($page.params.systemName !== user.login)
            throw error(404)

        await fetchTickets()
    })
</script>

<div class="divider divider">Данные пользователя</div>
<div class="flex flex-col content-center justify-center">
    <div class="overflow-x-auto flex justify-center">
        <div class="flex flex-col space-y-4">

            {#await promise}
                loading
            {:then user}
                <table class="table w-auto rounded-s">
                    <tbody>
                    <tr class="border border-primary/25">
                        <td class="font-bold">Фамилия</td>
                        <td class="font-mono">{user.surname}</td>
                    </tr>
                    <tr class="border border-primary/25">
                        <td class="font-bold">Имя</td>
                        <td class="font-mono font-light">{user.name}</td>
                    </tr>
                    <tr class="border border-primary/25">
                        <td class="font-bold">Отчество</td>
                        <td class="font-mono font-light">{user.patronymic}</td>
                    </tr>

                    <tr class="border border-primary/25">
                        <td class="font-bold">Номер паспорта</td>
                        <td class="font-mono font-light">{user.passportNumber}</td>
                    </tr>

                    <tr class="border border-primary/25">
                        <td class="font-bold">Логин</td>
                        <td class="font-mono font-light">{user.login}</td>
                    </tr>
                    </tbody>
                </table>
                <UpdateUserModal modalId="update_user_modal" user={user} onUpdated={() => promise = getUserData()}/>
            {/await}

            <button class="btn btn-primary" on:click={() => {
                document.getElementById(`update_user_modal`).showModal()
            }}>
                Редактировать
            </button>
        </div>
    </div>
</div>

<div class="divider divider">Билеты</div>

<div class="grid overflow-hidden px-20 pb-20">

    <div class="overflow-x-auto space-y-5 flex flex-col justify-center">
        <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2">
            <thead>
            <tr class="bg-primary text-white">
                <th>Название</th>
                <th>Номер билета</th>
                <th>Номер места</th>
                <th>Авиакомпания</th>
                <th>Время вылета</th>
                <th>Время прибытия</th>
            </tr>
            </thead>
            <tbody>
            {#each tickets as ticket}
                {@const departureDate = new Date(ticket.flight.departureDateTime)}
                {@const arrivalDate = new Date(ticket.flight.arrivalDateTime)}
                {@const flightAlreadyDeparted = departureDate < Date.now()}
                <tr class={flightAlreadyDeparted ? 'text-red-700': ''}>
                    <td>{ticket.title}</td>
                    <td>{ticket.ticketNumber}</td>
                    <td>{ticket.seatNumber}</td>
                    <td>
                        {ticket.flight.airline.title}
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
                </tr>
            {/each}
            {#if tickets.length === 0}
                <span class="p-4">Ничего не найдено</span>
            {/if}
            </tbody>
        </table>
    </div>
</div>

<!-- TODO: Can't delete user cos FK -->
<!-- TODO: Products view -->
<!-- TODO: Authorize -->
<!-- TODO: Pay system? -->

