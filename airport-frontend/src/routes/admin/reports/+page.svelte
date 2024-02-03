<script lang="ts">
    import UsersPerDayPlot from '$lib/components/UsersPerDayPlot.svelte'
    import FlightsPerDayPlot from '$lib/components/FlightsPerDayPlot.svelte'
    import PurchasesPerDayPlot from '$lib/components/PurchasesPerDayPlot.svelte'
</script>


<div class="flex flex-col space-y-5">
    <h1 class="text-2xl font-bold">Отчеты</h1>
    <p>Тут вы можете просматривать отчеты.</p>
    <div class="flex flex-col content-center">
        <div class="divider">Количество регистраций в день</div>
        <div class="w-3/4 self-center">
            <UsersPerDayPlot/>
        </div>
        <div class="divider">Количество рейсов в день</div>
        <div class="w-3/4 self-center">
            <FlightsPerDayPlot/>
        </div>
        <div class="divider">Количество продаж в день</div>
        <div class="w-3/4 self-center">
            <PurchasesPerDayPlot/>
        </div>
    </div>
</div>

