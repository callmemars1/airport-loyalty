<script lang="ts">
    import VnukovoLogo from '$lib/components/VnukovoLogo.svelte'
    import CartLogo from '$lib/components/CartLogo.svelte'
    import {isAuthenticated} from '$lib/scripts/auth'

    let bonusPoints: number = 42069
</script>


<div class="navbar bg-base-200 shadow-lg shadow-primary/50 sticky top-0 z-10">
    <div class="flex-1 gap-x-5">
        <a class="btn btn-ghost text-xl" href="/">
            <VnukovoLogo/>
        </a>
        <div class="flex-none">
            {#if $isAuthenticated}
                <ul class="menu menu-horizontal px-1 space-x-5 items-center">
                    <li>
                        <details>
                            <summary class="bg-primary/25">
                                Купить
                            </summary>
                            <ul class="p-2 bg-base-100 shadow-lg shadow-primary/50">
                                <li><a href="/flights/buy">Билеты</a></li>
                                <li><a>Услуги</a></li>
                            </ul>
                        </details>
                    </li>
                    <li>
                        <a>
                            <div class="indicator ">
                                <span class="indicator-item badge bg-primary/50 rounded-md badge-primary text-xs badge-xs">0</span>
                                <CartLogo/>
                            </div>
                        </a>
                    </li>
                </ul>
            {/if}
        </div>
    </div>

    <div class="flex-none">
        <ul class="menu menu-horizontal space-x-5 items-center">
            {#if $isAuthenticated}
                <li><a href="/user/pavel">Профиль</a></li>
                <li class="bg-primary/25">
                <span class="">
                    Бонусы: <span class="font-mono">{bonusPoints}</span>б.
                </span>
                </li>
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