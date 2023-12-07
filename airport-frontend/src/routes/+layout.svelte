<script lang="ts">
    import './app.css'
    import { onMount, onDestroy } from 'svelte';
    import { checkAuthorized, isAuthenticated } from '$lib/scripts/auth';
    
    
    onMount(() => {
        setupAuthorizedStore()
        setupAuthorizationCheckUps()
    });

    let saveStore = false
    $: if (saveStore && $isAuthenticated) {
        window.sessionStorage.setItem("isAuthenticatedStore", JSON.stringify($isAuthenticated))
    }
    
    const setupAuthorizedStore = () => {
        let ses = window.sessionStorage.getItem("isAuthenticatedStore")
        if (ses) {
            $isAuthenticated = JSON.parse(ses)
        }
        saveStore = true
    }
    
    const setupAuthorizationCheckUps = () => {
        // Check authorization immediately when the component mounts
        checkAuthorized();

        // Set up an interval to check authorization every 5 seconds
        let interval = setInterval(checkAuthorized, 5000);

        onDestroy(() => {
            // Clear the interval when the component is destroyed to prevent memory leaks
            clearInterval(interval);
        });
    }
</script>


<slot />