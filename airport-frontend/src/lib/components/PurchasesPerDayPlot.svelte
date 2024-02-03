<script lang="ts">
    import {getPurchasesPerDateStatistics} from '$lib/scripts/statistics'
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
                name: "Продаж сделано",
                data: series,
            },
        ],
        xaxis: {
            categories: categories
        },
    };
    onMount(async () =>{
        let statistics = await getPurchasesPerDateStatistics()

        series = statistics.map((v) => v.unitsCount);
        categories = statistics.map((v) => v.date);

        options = {
            legend: {
                show: false,
            },
            chart: {
                type: "bar",
            },
            series: [
                {
                    name: "Продаж сделано",
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