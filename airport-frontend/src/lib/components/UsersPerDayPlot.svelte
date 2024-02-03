<script lang="ts">
    import {getUsersCountPerDateStatistics} from '$lib/scripts/statistics'
    import { chart } from "svelte-apexcharts";
    import { onMount } from "svelte"


    let series: Array = []
    let categories: Array = []
    let options = {
        chart: {
            type: "bar",
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
        let statistics = await getUsersCountPerDateStatistics()

        series = statistics.map((v) => v.unitsCount);
        categories = statistics.map((v) => v.date);

        options = {
            chart: {
                type: "bar",
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
    })
</script>

<div use:chart={options} />