<script lang="ts">
    export let title: String = "No title.";
    export let id: String;
    export let placeholder: String = title.toLowerCase();
    export let inputType: String = "text";
    export let value: String;
    export let errors: string[];

    $: valid = !errors || errors.length === 0


    export let required: Boolean = false;
</script>


<div class="mb-4">
    <div class="tooltip" data-tip="Обязательное поле">
        <label class="block text-sm font-bold mb-2" for={id}>
            {title}
            {#if required}
                <sup class="text-primary text-l">*</sup>
            {/if}
        </label>
    </div>
    {#if inputType === "text"}
        <input
                class="input input-bordered w-full"
                class:border-red-700={!valid}
                id={id}
                placeholder={placeholder}
                type="text"
                required={required}
                bind:value={value}
        />
    {:else if inputType === "password"}
        <input
                class="input input-bordered w-full"
                class:border-red-700={!valid}
                id={id}
                placeholder={placeholder}
                type="password"
                required={required}
                bind:value={value}
        >
    {/if}

    {#if !valid}
        <div class="flex flex-col">
            {#each errors as error}
                <span class="mt-2 text-sm text-red-500">
                    — {error}
                </span>
            {/each}
        </div>
    {/if}
</div>