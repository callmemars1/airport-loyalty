<script lang="ts">
    import {goto} from '$app/navigation';
    import Form from '$lib/components/Form.svelte';
    import Preloader from '$lib/components/Preloader.svelte';
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'
    import { PassportNumberFormatter } from '$lib/scripts/PassportNumberFormatter';

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

    const onPassportNumberChange = (newPassportNumber: string) => {
        console.log('Passport number changed:', newPassportNumber);
    };
    
    const formatter = new PassportNumberFormatter(passportNumber, onPassportNumberChange);


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

<Form title="Регистрация" submitEventName="signUpFormSubmit" on:signUpFormSubmit={() => submitPromise = handleSubmit()}>
    <svelte:fragment slot="inputs">
        <div class="divider divider-neutral">Персональные данные</div>
        <LabeledFormInput
                title="Фамилия"
                id="surname"
                placeholder="Иванов"
                bind:value={surname}
                bind:errors={errors.surname}
                required
        />
        <div class="flex flex-row gap-2">
            <LabeledFormInput
                    title="Имя"
                    id="name"
                    placeholder="Иван"
                    bind:value={name}
                    errors={errors.name}
                    required
            />
            <LabeledFormInput
                    title="Отчество"
                    id="patronymic"
                    placeholder="Иванович"
                    bind:value={patronymic}
                    errors={errors.patronymic}
            />
        </div>
        <LabeledFormInput
                title="Номер паспорта"
                id="passportNumber"
                placeholder="1234 567890"
                inputType="text"
                bind:value={passportNumber}
                errors={errors.passportNumber}
                formatter={(e) => formatter.format(e)}
                required
        />
        <div class="divider divider-neutral">Данные для входа</div>
        <LabeledFormInput
                title="Логин"
                id="login"
                placeholder="ivanov1995"
                bind:value={login}
                bind:errors={errors.login}
                required
        />
        <LabeledFormInput
                title="Пароль"
                id="password"
                placeholder="********"
                inputType="password"
                bind:value={password}
                errors={errors.password}
                required
        />
    </svelte:fragment>
    <svelte:fragment slot="controls">
        <a class="btn btn-ghost" href="/">Назад</a>
        <button class="btn btn-primary btn-outline" type="submit">Продолжить</button>
    </svelte:fragment>
</Form>
<span>Уже зарегистрированы? <a href="sign-in" class="link text-primary">Войдите.</a></span>

<Preloader bind:promise={submitPromise}/>
