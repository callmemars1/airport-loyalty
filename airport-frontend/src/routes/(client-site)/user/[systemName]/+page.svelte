<script lang="ts">
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'
</script>


<div class="divider divider">Данные пользователя</div>
<div class="flex flex-col content-center justify-center">
    <div class="overflow-x-auto flex justify-center">
        <div class="flex flex-col space-y-4">
            <table class="table w-auto rounded-s">
                <tbody>
                <tr class="border border-primary/25">
                    <td class="font-bold">Фамилия</td>
                    <td class="font-mono">Иванов</td>
                </tr>
                <tr class="border border-primary/25">
                    <td class="font-bold">Имя</td>
                    <td class="font-mono font-light">Павел</td>
                </tr>
                <tr class="border border-primary/25">
                    <td class="font-bold">Отчество</td>
                    <td class="font-mono font-light">Михайлович</td>
                </tr>

                <tr class="border border-primary/25">
                    <td class="font-bold">Номер паспорта</td>
                    <td class="font-mono font-light">1488 420228</td>
                </tr>

                <tr class="border border-primary/25">
                    <td class="font-bold">Логин</td>
                    <td class="font-mono font-light">pashka_kakashka</td>
                </tr>
                </tbody>
            </table>

            <button class="btn btn-outline btn-primary">
                Редактировать
            </button>
        </div>
    </div>
</div>

<div class="divider divider">Билеты</div>
<div class="divider divider">Услуги</div>
