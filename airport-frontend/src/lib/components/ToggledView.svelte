<script lang="ts">
    export let isToggled = false;
</script>

<div class="flex flex-row space-x-3">
    <div class="form-control w-52">
        <label class="cursor-pointer label">
            <span class="label-text font-bold">Прилеты</span>
            <input type="checkbox" class="toggle toggle-primary" bind:checked={isToggled} />
            <span class="label-text font-bold">Вылеты</span>
        </label>
    </div>
</div>