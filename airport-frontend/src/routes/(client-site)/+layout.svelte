<script>
    import Navbar from '$lib/components/Navbar.svelte'
    import Footer from '$lib/components/Footer.svelte'
</script>

<div class="flex flex-col space-between min-h-screen">
    <div class="flex flex-col flex-1 space-y-16">
        <Navbar />
        <slot/>
    </div>
    <Footer/>
</div>
