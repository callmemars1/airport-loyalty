<script lang="ts">
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'
    import {getAirports, getAirlines} from '$lib/scripts/api'
    import {formatDateTime} from "$lib/scripts/utils"
    import {getFlightByNumber, getServiceClassesForFlight} from "$lib/scripts/api"
    import {onMount} from 'svelte'

    export let modalId: string
    export let ticket: {}
    export let onEditEnded

    const updateProduct = async () => {
        const response = await fetch(`/api/products/change-price?productId=${ticket.id}&newPrice=${ticket.price}`, {
            method: 'PATCH',
        });

        if (response.status === 400) {
            const errorData = await response.json();
            console.log(errorData)
        } else if (response.ok) {
            closeModal()
        }
    }

    const handleSubmit = async () => {
        await updateProduct();
    };

    const closeModal = () => {
        document.querySelector(`#${modalId}`).close()
        onEditEnded()
    }

    const handleDelete = async () => {
        const response = await fetch(`/api/products/delete?productId=${ticket.id}`, {
            method: 'DELETE',
        });

        if (response.status === 400) {
            const errorData = await response.json();
            console.log(errorData)
        } else if (response.ok) {
            closeModal()
        }
    }

    $: canSendRequest = ticket.price


</script>

<dialog id={modalId} class="modal">
    <div class="modal-box w-full">
        <form method="dialog">
            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
        </form>
        <h3 class="font-bold text-lg">Редактировать билет</h3>
        <div class="flex flex-col space-y-6 mt-4">
            <div class="flex flex-col space-y-4">
                <div class="divider divider-neutral"><h1
                        class="block text-sm font-bold mb-2 font-mono">{ticket.title}</h1></div>
                <div class="flex flex-col content-center">
                    <label for="price" class="block text-sm font-bold mb-2">Стоимость, ₽</label>
                    <input type="number"
                           class="input input-bordered w-full p-1 text-sm"
                           bind:value={ticket.price}
                           min="0"
                           max="1000000"
                           id="price"
                    >
                </div>
            </div>

            <div class="flex flex-row justify-between">
                <button class="btn btn-ghost" on:click={closeModal}>Назад</button>
                <button class="btn btn-primary btn-outline" type="submit" on:click={handleSubmit}
                        disabled={!canSendRequest}>
                    Продолжить
                </button>
            </div>
            <div class="flex flex-col">
                <div class="divider divider-error p-0 m-0"></div>
                <p class="self-center text-gray-500 pb-3">Восстановить товар будет невозможно!</p>
                <button class="btn btn-error btn-outline w-full" type="submit" on:click={handleDelete}>
                    Отменить продажу товара
                </button>
            </div>
        </div>

    </div>
</dialog>