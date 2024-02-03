<script lang="ts">
    import VnukovoLogo from '$lib/components/VnukovoLogo.svelte'
    import {onMount} from 'svelte'
    import LogOutIcon from '$lib/components/LogOutIcon.svelte'
    import {signOut} from '$lib/scripts/auth'
    import {getUserData} from '$lib/scripts/api'
    import {goto} from '$app/navigation'

    export let role: string | undefined

    let userLogin = ''
    let userData = {}

    onMount(async () => {
        userData = await getUserData()
        userLogin = userData.login
    })
</script>


<div class="navbar shadow-md shadow-primary/50 sticky top-0 z-50 bg-base-100">
    <div class="flex-1 gap-x-5">
        <a class="btn btn-ghost text-xl" href="/">
            <VnukovoLogo/>
        </a>
        <div class="flex-none">
            {#if role}
                <ul class="menu menu-horizontal px-1 space-x-5 items-center">
                    <li>
                        <details>
                            <summary class="bg-primary/25">
                                Купить
                            </summary>
                            <ul class="p-2 bg-base-100 shadow-lg shadow-primary/50">
                                <li><a href="/flights/buy">Билеты</a></li>
                            </ul>
                        </details>
                    </li>
                </ul>
            {/if}
        </div>
    </div>

    <div class="flex-none">
        <ul class="menu menu-horizontal space-x-5 items-center">
            {#if role}
                <li>
                    <a class="btn btn-outline btn-success" href="/balance/add">
                        <span class="font-mono"><span class="text-lg">₽</span> {userData.balance}</span>
                    </a>
                </li>
                <li><a href="/user/{userLogin}">Профиль</a></li>
                <button
                        class="btn btn-outline btn-primary"
                        on:click={async () => {
                                await signOut()
                                await goto('/auth/sign-in')
                            }}
                >
                    <LogOutIcon/>
                </button>
            {:else }
                <li class="btn btn-primary">
                    <a class="hover:bg-inherit hover:text-inherit" href="/auth/sign-in">
                        Войти
                    </a>
                </li>
                <li class="btn btn-outline btn-primary">
                    <a class="hover:bg-inherit hover:text-inherit" href="/auth/sign-up">
                        Зарегистрироваться
                    </a>
                </li>
            {/if}
        </ul>
    </div>
</div>