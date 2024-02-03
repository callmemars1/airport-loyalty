<script lang="ts">
    import {onMount} from "svelte";
    import {getUsers, PageSize, deleteUserById} from "$lib/scripts/api";
    import TrashIcon from "$lib/components/TrashIcon.svelte";
    import EditLogo from "$lib/components/EditLogo.svelte";
    import CreateUserModal from '$lib/components/CreateUserModal.svelte'
    import UpdateUserModal from '$lib/components/UpdateUserModal.svelte'

    let users = [];
    export let usersLoading = true;

    let pageNumber = 1

    async function fetchUsers() {
        usersLoading = true;
        users = await getUsers(pageNumber)
        usersLoading = false;
    }

    onMount(async () => {
        await fetchUsers()
    });

    let formatTwoDigit = (num: number) => {
        if (num % 10 === num)
            return `0${num}`
        return num.toString()
    }

    let formatDateTime = (date: Date) => {
        return `${formatTwoDigit(date.getHours())}:${formatTwoDigit(date.getMinutes())} — ${formatTwoDigit(date.getDate())}.${formatTwoDigit(date.getMonth() + 1)}.${date.getFullYear()}`
    }


    let file;
    let errors = []

    async function uploadFile() {
        if (!file) {
            alert('Please select a file.');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch('/api/users/upload', {
                method: 'POST',
                body: formData
            });

            if (response.status === 400) {
                let body = await response.json()
                errors = body.errors
                console.log(errors)
            } else if (!response.ok) {
                throw new Error(`Server responded with ${response.status}`);
            }

            await fetchUsers()
        } catch (err) {
            console.error('Error uploading file:', err);
            alert('Error uploading file.');
        }
    }

    function handleFileChange(event) {
        const files = event.target.files;
        if (files.length > 0) {
            const selectedFile = files[0];
            if (selectedFile.type === "application/json") {
                file = selectedFile;
            } else {
                alert('Please select a JSON file.');
                file = null;
            }
        }
    }
</script>

<CreateUserModal modalId="create_user_modal" onCreate={() => fetchUsers()}/>

<div class="flex flex-col space-y-10">
    <div class="flex flex-col space-y-5">
        <div class="flex flex-row justify-between">
            <div class="flex flex-col space-y-5">
                <h1 class="text-2xl font-bold">Пользователи</h1>
                <p>Тут вы можете просматривать информацию о пользователях, а также редактировать их.</p>
            </div>
            <button class="btn btn-success self-end" on:click={() => {
                document.getElementById(`create_user_modal`).showModal()
            }}>Создать пользователя
            </button>
        </div>
        <label class="form-control w-full">
            <div class="label">
                <span class="label-text">Импортировать пользователей</span>
            </div>
            <div class="flex flex-row space-x-16">
                <input type="file" accept=".json" class="file-input file-input-bordered file-input-primary w-full"
                       on:change={handleFileChange}/>
                <button class="btn btn-success self-end" disabled={!file} on:click={uploadFile}>Импортировать</button>
            </div>
        </label>
        {#if errors}
            <ul class="space-y-4 text-red-700 list-disc list-inside">
                {#each Object.entries(errors) as [key, values]}
                    <li>
                        Поле: "{key}"
                        <ol class="ps-5 mt-2 space-y-1 list-decimal list-inside">
                            {#each values as value}
                                <li>{value}</li>
                            {/each}
                        </ol>
                    </li>
                {/each}
            </ul>
        {/if}

    </div>

    <div class="divider">Список пользователей</div>

    <div class="flex flex-col space-y-5 pb-5">
        <div class="grid overflow-hidden">

            <div class="overflow-x-auto space-y-5 flex flex-col justify-center">
                <table class="table table-zebra w-full border border-primary col-start-1 col-end-2 row-start-1 row-end-2">
                    <thead>
                    <tr class="bg-primary text-white">
                        <th></th>
                        <th>Логин</th>
                        <th>ФИО</th>
                        <th>Роль</th>
                        <th>Дата регистрации</th>
                        <th>Номер паспорта</th>
                        <th></th>
                    </tr>
                    </thead>
                    <tbody>
                    {#each users as user (user)}
                        {@const date = new Date(user.createdAt)}
                        {@const modalId = `update_user_modal_${user.login}`}
                        <tr>
                            <td>
                                {#if user}
                                    <button on:click={() => {
                                        document.getElementById(`update_user_modal_${user.login}`).showModal()
                                        user = user
                                    }}>
                                        <EditLogo/>
                                    </button>
                                {/if}
                            </td>
                            <td>{user.login}</td>
                            <td>
                                {user.surname} {user.name} {user === null ? user.patronymic : ''}
                            </td>
                            <td>{user.role}</td>
                            <td>
                                {formatDateTime(date)}
                            </td>
                            <td>{user.passportNumber}</td>
                            <td>
                                <button class="btn btn-error" on:click={async () => {
                                    await deleteUserById(user.id)
                                    users.splice(users.indexOf(user), 1)
                                    users = users
                                }}>
                                    <TrashIcon/>
                                </button>
                            </td>
                            {#if user}
                                <UpdateUserModal modalId={modalId} user={user} onUpdated={() => {fetchUsers()}}/>
                            {/if}
                        </tr>
                    {/each}
                    {#if users.length === 0}
                        <span class="p-4">Ничего не найдено</span>
                    {/if}
                    </tbody>
                </table>

                <div class="join self-center">
                    <button
                            class="join-item btn"
                            on:click={async () => {
                            pageNumber = pageNumber - 1
                            await fetchUsers()
                        }}
                            disabled={pageNumber <= 1}
                    >
                        «
                    </button>
                    <button class="join-item btn">{pageNumber}</button>
                    <button
                            class="join-item btn"
                            on:click={async () => {
                            pageNumber = pageNumber + 1
                            await fetchUsers()
                        }}
                            disabled={users.length - PageSize < 0}
                    >
                        »
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>