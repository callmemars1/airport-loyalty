<script lang="ts">
    import { page } from '$app/stores';
    import "./app.css";
</script>

<div class="h-screen flex items-center justify-center">
    <div class="text-center space-y-6">
        <h1 class="text-4xl font-bold">{$page.status}</h1>
        <h2 class="text-xl font-semibold">{$page.error.message}</h2>
        <p class="text-m">Что-то пошло не так :(</p>
        <button class="btn btn-primary" onclick="window.history.back()">Назад</button>
    </div>
</div>