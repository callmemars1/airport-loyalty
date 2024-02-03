<script lang="ts">
    import type { PageData } from './$types';
    
    export let data: PageData
</script>


<div class="flex flex-col space-y-5">
    <h1 class="text-2xl font-bold">Страница администрации</h1>
    <p>Тут вы можете просматривать отчеты, а также редактировать данные.</p>
    <p>{data.roleSystemName}</p>
</div>