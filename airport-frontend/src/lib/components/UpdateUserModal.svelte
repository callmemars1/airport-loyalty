<script lang="ts">
    import LabeledFormInput from "$lib/components/LabeledFormInput.svelte";
    import PassportNumberFormatter from '$lib/scripts/passportNumberFormatter'
    import ErrorsList from '$lib/components/ErrorsList.svelte'
    import {getRoles, getUserDataById} from '$lib/scripts/api'
    import {onMount} from 'svelte'

    export let modalId: string
    export let user
    export let onUpdated
    export let userAdmin


    $: id = user.id
    $: name = user.name
    $: surname = user.surname
    $: patronymic = user.patronymic
    $: passportNumber = user.passportNumber
    $: role = userAdmin ? roleObject.systemName : null
    
    let password = ''
    
    
    let roleObject = {}
    let roles = []

    
    onMount(async () => {
        if (userAdmin){
            roles = await getRoles();
            roleObject = roles.find((r) => r.systemName === user.role)
        }
    })

    
    let errors = {}
    const updateUser = async (data) => {
        const response = await fetch(`/api/users/update`, {
            method: 'PUT',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(data),
        });

        if (response.status === 400) {
            const errorData = await response.json();
            errors = errorData.errors;
            console.log(errors)
        } else if (response.ok) {
            document.querySelector(`#${modalId}`).close()
            onUpdated()
        } else {
            throw new Error(response.status)
        }
    };

    const onPassportNumberChange = (newPassportNumber: string) => {
        console.log('Passport number changed:', newPassportNumber);
        passportNumber = newPassportNumber
    };

    const formatter = new PassportNumberFormatter(passportNumber, onPassportNumberChange);

    const handleSubmit = async () => {
        await updateUser({
            id,
            name,
            surname,
            patronymic,
            passportNumber,
            password,
            role
        });
    };

    let submitPromise: Promise
</script>

<dialog id={modalId} class="modal">
    <div class="modal-box w-full">
        <form method="dialog">
            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
        </form>
        <h3 class="font-bold text-lg">Обновить пользователя</h3>
        <div class="flex flex-col w-full">
            <div class="divider divider-neutral">Персональные данные</div>
            <LabeledFormInput
                    title="Фамилия"
                    id="surname"
                    placeholder="Иванов"
                    bind:value={user.surname}
                    bind:errors={errors.surname}
                    required
            />
            <div class="flex flex-row gap-2">
                <LabeledFormInput
                        title="Имя"
                        id="name"
                        placeholder="Иван"
                        bind:value={user.name}
                        errors={errors.name}
                        required
                />
                <LabeledFormInput
                        title="Отчество"
                        id="patronymic"
                        placeholder="Иванович"
                        bind:value={user.patronymic}
                        errors={errors.patronymic}
                />
            </div>
            <LabeledFormInput
                    title="Номер паспорта"
                    id="passportNumber"
                    placeholder="1234 567890"
                    inputType="text"
                    bind:value={user.passportNumber}
                    errors={errors.passportNumber}
                    formatter={(e) => formatter.format(e)}
                    required
            />
            {#if userAdmin}
                <label for="role-select" class="block text-sm font-bold mb-2">Роль</label>
                <select id="role-select" class="select select-bordered w-full" bind:value={roleObject}>
                    {#each roles as optionRole}
                        <option value={optionRole}>
                            {optionRole.title}
                        </option>
                    {/each}
                </select>
            {/if}
            {#if !errors.role}
                <ErrorsList bind:errors={errors.role}/>
            {/if}
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
                <a class="btn btn-ghost" href="/">Назад</a>
                <button class="btn btn-primary btn-outline" type="submit" on:click={handleSubmit}>Продолжить
                </button>
            </div>
        </div>
    </div>
</dialog>