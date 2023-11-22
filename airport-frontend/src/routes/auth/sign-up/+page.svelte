<script lang="ts">
    import {goto} from '$app/navigation';
    import Form from '$lib/components/Form.svelte';
    import Preloader from '$lib/components/Preloader.svelte';
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'

    let name = '';
    let surname = '';
    let patronymic = '';
    let passportNumber = '';
    let login = '';
    let password = '';

    interface Errors {
        [key: string]: string[];
    }

    let errors: Errors = {}

    const signUp = async (data) => {
        await new Promise( resolve => setTimeout(resolve, 5000));
        const response = await fetch('/api/auth/sign-up', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        });

        if (response.status === 400) {
            const errorData = await response.json();
            errors = errorData.errors;
        } else if (response.status === 201) {
            await goto('/auth/sign-in')
        } else {
            throw new Error(response.status)
        }
    };

    const handleSubmit = async () => {
        await signUp({
            name,
            surname,
            patronymic,
            passportNumber,
            login,
            password,
        });
    };
    
    let submitPromise: Promise
</script>

<Form title="Регистрация" on:formSubmit={() => submitPromise = handleSubmit()}>
    <svelte:fragment slot="inputs">
        <div class="divider divider-neutral">Персональные данные</div>
        <LabeledFormInput title="Фамилия" id="surname" bind:value={surname} bind:errors={errors.surname} required/>
        <div class="flex flex-row gap-2">
            <LabeledFormInput title="Имя" id="name" bind:value={name} required errors={errors.name}/>
            <LabeledFormInput title="Отчество" id="patronymic" bind:value={patronymic} errors={errors.patronymic}/>
        </div>
        <LabeledFormInput title="Номер паспорта" id="passportNumber" bind:value={passportNumber} required
                          errors={errors.passportNumber}/>
        <div class="divider divider-neutral">Данные для входа</div>
        <LabeledFormInput title="Логин" id="login" bind:value={login} required bind:errors={errors.login}/>
        <LabeledFormInput title="Пароль" id="password" inputType="password" bind:value={password} required
                          errors={errors.password}/>
    </svelte:fragment>
    <svelte:fragment slot="controls">
        <button class="btn btn-ghost" type="submit">Назад</button>
        <button class="btn btn-primary btn-outline" type="submit">Продолжить</button>
    </svelte:fragment>
</Form>
<span>Уже зарегистрированы? <a href="sign-in" class="link text-primary">Войдите.</a></span>

<Preloader bind:promise={submitPromise} />
