<script lang="ts">
    import { fade, scale } from "svelte/transition";

    export let promise:Promise
</script>


{#await promise}
    <!-- promise is pending -->
    <span class="loading loading-ring loading-lg"></span>
{:then value}
    <!-- promise was fulfilled or not a Promise -->
    <div class="text-green-700" in:scale out:fade>
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-15 h-15">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
    </div>
{:catch error}
    <div class="text-red-700" in:scale out:fade>
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-15 h-15">
            <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
    </div>
{/await}