<script lang="ts">
    import Form from "$lib/components/Form.svelte"
    import LabeledFormInput from "$lib/components/LabeledFormInput.svelte"

    let login: String = ""
    let password: String = ""
    
    let errors = [""]
</script>

<Form title="Вход">
    <svelte:fragment slot="inputs">
        <LabeledFormInput title="Логин" bind:value={login} bind:errors={errors} required/>
        <LabeledFormInput title="Пароль" inputType="password" bind:errors={errors} bind:value={password} required/>
    </svelte:fragment>
    <svelte:fragment slot="controls">
        <button class="btn btn-ghost" type="submit">Назад</button>
        <button class="btn btn-primary btn-outline" type="submit">Продолжить</button>
    </svelte:fragment>
</Form>
<span>Еще нет аккаунта? <a href="sign-up" class="link text-primary">Зарегистрируйтесь.</a></span>