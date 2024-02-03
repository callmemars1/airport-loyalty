<script>
    import Navbar from '$lib/components/Navbar.svelte'
    import Footer from '$lib/components/Footer.svelte'
    
    export let data;
</script>

<div class="flex flex-col space-between min-h-screen">
    <div class="flex flex-col flex-1 space-y-16">
        <Navbar role={data.roleSystemName}/>
        <slot/>
    </div>
    <Footer userAdmin={data.roleSystemName && data.roleSystemName !== 'Client'}/>
</div>
