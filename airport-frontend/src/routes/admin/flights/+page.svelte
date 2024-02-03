<script lang="ts">
    import CreateFlightModal from "$lib/components/CreateFlightModal.svelte"
    import FlightsSearch from "$lib/components/FlightsSearch.svelte"
</script>


<div class="flex flex-col space-y-10">
    <div class="flex flex-row justify-between">
        <div class="flex flex-col space-y-5">
            <h1 class="text-2xl font-bold">Рейсы</h1>
            <p>Тут вы можете просматривать информацию о рейсах, а также редактировать их.</p>
        </div>
        <button class="btn btn-success self-end"  onclick="create_flight_modal.showModal()">Зарегистрировать рейс</button>
    </div>

    <div class="divider">Список рейсов</div>

    <FlightsSearch />
</div>


<CreateFlightModal modalId="create_flight_modal"/>