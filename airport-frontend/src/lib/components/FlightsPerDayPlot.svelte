<script lang="ts">
    import {getFlightsPerDateStatistics} from '$lib/scripts/statistics'
    import { chart } from "svelte-apexcharts";
    import { onMount } from "svelte"


    let series: Array = []
    let categories: Array = []
    let options = {
        legend: {
            show: false
        },
        chart: {
            type: "line",
        },
        series: [
            {
                name: "Пользователей зарегистрировано",
                data: series,
            },
        ],
        xaxis: {
            categories: categories
        },
    };
    onMount(async () =>{
        let statistics = await getFlightsPerDateStatistics()

        series = statistics.map((v) => v.unitsCount);
        categories = statistics.map((v) => v.date);

        options = {
            legend: {
                show: false,
            },
            chart: {
                type: "line",
            },
            series: [
                {
                    name: "Рейсов назначено",
                    data: series,
                },
            ],
            xaxis: {
                categories: categories
            },
        };
    })
</script>

<div use:chart={options} />