<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    
    export let submitEventName: string

    const dispatch = createEventDispatcher();

    function handleSubmit(event) {
        event.preventDefault();
        dispatch(submitEventName, event); // Dispatch the custom 'submit' event with the event object
    }

    export let title: String = "No title.";
</script>

<form class="shadow-primary/25 shadow-xl border rounded px-8 pt-6 pb-8 mb-4 w-full md:w-1/2 lg:w-1/3" 
      novalidate 
      on:submit={handleSubmit}
>
    <h2 class="text-center text-2xl mb-6">{title}</h2>
    <slot name="inputs" />
    <div class="flex items-center justify-between">
        <slot name="controls"/>
    </div>
</form>