<script lang="ts">
    import { onDestroy } from 'svelte'
    import { isAuthenticated } from '$lib/scripts/auth'
    import { goto } from '$app/navigation'

    let signedIn = $isAuthenticated
    $: {
        if (!signedIn)
            goto('/auth/sign-in')
    }
    let unsubscribe = isAuthenticated.subscribe((value) => {
        signedIn = value
    })
    onDestroy(unsubscribe);
</script>