<script lang="ts">
    export let errors: string[] = []
</script>

<div class="flex flex-col">
    {#each errors as error}
        <span class="mt-2 text-sm text-red-500">
            — {error}
        </span>
    {/each}
</div>