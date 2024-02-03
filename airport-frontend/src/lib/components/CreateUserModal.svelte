<script lang="ts">
    import LabeledFormInput from "$lib/components/LabeledFormInput.svelte";
    import PassportNumberFormatter from '$lib/scripts/passportNumberFormatter'
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import {getRoles} from '$lib/scripts/api'
    import {onMount} from 'svelte'

    export let modalId: string
    export let onCreate

    let surname = ''
    let name = ''
    let patronymic = ''
    let passportNumber = ''
    let login = ''
    let password = ''
    let roleObject = {}
    let roles = []

    $: role = roleObject.systemName

    interface Errors {
        [key: string]: string[];
    }

    let errors: Errors = {}

    onMount(async () => {
        roles = await getRoles();
        roleObject = roles.find((r) => r.systemName === 'Client')
    })

    const closeModal = () => document.querySelector(`#${modalId}`).close()

    const createUser = async (data) => {
        const response = await fetch('/api/auth/create-staff', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        });

        if (response.status === 400) {
            let errorData = await response.json();
            errors = {...errorData.errors};
            console.log(errors)
        } else if (response.status === 201) {
            document.querySelector(`#${modalId}`).close()
            onCreate()
        } else {
            throw new Error(response.status)
        }
    };

    const onPassportNumberChange = (newPassportNumber: string) => {
        console.log('Passport number changed:', newPassportNumber);
        passportNumber = newPassportNumber
    };

    const formatter = new PassportNumberFormatter(passportNumber, onPassportNumberChange);

    const handleSubmit = async (e) => {
        e.preventDefault()
        await createUser({
            name,
            surname,
            patronymic,
            passportNumber,
            login,
            password,
            role
        });
    };
</script>

<dialog id={modalId} class="modal">
    <div class="modal-box w-full">
        <form method="dialog">
            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
        </form>
        <h3 class="font-bold text-lg">Создать пользователя</h3>
        <div class="flex flex-col w-full">
            <div class="divider divider-neutral">Персональные данные</div>
            <LabeledFormInput
                    title="Фамилия"
                    id="surname"
                    placeholder="Иванов"
                    bind:value={surname}
                    bind:errors={errors['surname']}
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
            <label for="role-select" class="block text-sm font-bold mb-2">Роль</label>
            <select id="role-select" class="select select-bordered w-full" bind:value={roleObject}>
                {#each roles as optionRole}
                    <option value={optionRole}>
                        {optionRole.title}
                    </option>
                {/each}
            </select>
            {#if errors.role}
                <ErrorsList errors={errors.role}/>
            {/if}
            <div class="divider divider-neutral">Данные для входа</div>
            <LabeledFormInput
                    title="Логин"
                    id="login"
                    placeholder="ivanov1995"
                    bind:value={login}
                    errors={errors.login}
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
            <div class="flex flex-row justify-between">
                <button class="btn btn-ghost" on:click={closeModal}>Назад</button>
                <button class="btn btn-primary btn-outline" on:click={handleSubmit}>
                    Продолжить
                </button>
            </div>
        </div>
    </div>
</dialog>
