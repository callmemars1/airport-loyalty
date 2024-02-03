<script lang="ts">
    import Form from "$lib/components/Form.svelte"
    import LabeledFormInput from "$lib/components/LabeledFormInput.svelte"
    import Preloader from '$lib/components/Preloader.svelte'
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import {signIn as signInApiCall} from '$lib/scripts/auth'
    import { goto } from '$app/navigation'

    let login: String = ""
    let password: String = ""
    let errors: string[] = []

    let submitPromise: Promise

    const signIn = async () => {
        errors = await signInApiCall(login, password)
        if (errors.length === 0) {
            await goto('/')
        }
    };

    const handleSubmit = async () => {
        await signIn();
    };

</script>

<Form title="Вход" submitEventName="signInFormSubmit" on:signInFormSubmit={() => submitPromise = handleSubmit()}>
    <svelte:fragment slot="inputs">
        <LabeledFormInput
                title="Логин"
                placeholder="ivanov01"
                bind:value={login}
                required
        />
        <LabeledFormInput
                title="Пароль"
                placeholder="********"
                inputType="password"
                bind:value={password}
                required
        />
        {#if errors && errors.length > 0}
            <div class="divider divider-neutral">Ошибка</div>
            <ErrorsList bind:errors={errors} />
            <div class="divider divider-neutral" />
        {/if}
    </svelte:fragment>
    <svelte:fragment slot="controls">
        <a class="btn btn-ghost" href="/">Назад</a>
        <button class="btn btn-primary btn-outline" type="submit">Продолжить</button>
    </svelte:fragment>
</Form>
<span>Еще нет аккаунта? <a href="sign-up" class="link text-primary">Зарегистрируйтесь.</a></span>

<Preloader bind:promise={submitPromise}/>