<script lang="ts">
    import ErrorsList from './ErrorsList.svelte'
    
    export let title: String = "No title.";
    export let id: String;
    export let placeholder: String = title.toLowerCase();
    export let inputType: 'email' | 'text' | 'password' | 'number' = 'text';
    export let value: String;
    export let errors: string[];
    export let required: Boolean = false;
    
    export let formatter = (e: InputEvent) => {  };
    
    const ref = (node: HTMLInputElement) => {
        node.type = inputType
    }

    $: valid = !errors || errors.length === 0
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
    <input
            class="input input-bordered w-full placeholder-secondary/50"
            class:border-red-700={!valid}
            id={id}
            placeholder={placeholder}
            required={required}
            bind:value={value}
            use:ref
            on:beforeinput={formatter}
    />

    {#if !valid}
        <ErrorsList bind:errors={errors} />
    {/if}
</div>