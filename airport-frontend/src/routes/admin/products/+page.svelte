<script lang="ts">
    import CreateTicketProduct from "$lib/components/CreateTicketProduct.svelte"
    import FlightsBuySearch from "$lib/components/FlightsBuySearch.svelte"
</script>


<div class="flex flex-col space-y-10">
    <div class="flex flex-row justify-between">
        <div class="flex flex-col space-y-5">
            <h1 class="text-2xl font-bold">Товары</h1>
            <p>Тут вы можете просматривать информацию о товарах, а также редактировать их.</p>
        </div>
        <button class="btn btn-success self-end"  onclick="create_ticket_modal.showModal()">Создать товар</button>
    </div>

    <div class="divider">Список товаров</div>

    <FlightsBuySearch adminPage/>
</div>


<CreateTicketProduct modalId="create_ticket_modal"/>