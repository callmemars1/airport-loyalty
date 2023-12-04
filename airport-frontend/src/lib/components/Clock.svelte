<script lang="ts">
    import { onMount } from "svelte";

    let currentTime = new Date().toLocaleTimeString();

    // Updates the current time every second
    onMount(() => {
        const interval = setInterval(() => {
            currentTime = formatDateTime(new Date())
        }, 1000);

        return () => {
            clearInterval(interval); // Cleanup the interval when the component is destroyed
        };
    });

    let formatTwoDigit = (num: number) => {
        if (num % 10 === num)
            return `0${num}`
        return num.toString()
    }

    let formatDateTime = (date: Date) => {
        return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())}:${formatTwoDigit(date.getSeconds())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
    }
</script>

<div class="flex items-center justify-center">
    <div class="text-4xl font-semibold">
        {currentTime}
    </div>
</div>