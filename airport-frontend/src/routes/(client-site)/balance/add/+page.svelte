<script>
    import Form from '$lib/components/Form.svelte'
    import LabeledFormInput from '$lib/components/LabeledFormInput.svelte'

    let sumToAdd = 0

    const handleSubmit = async () => {
        let result = await fetch(`/api/users/add-balance?sumToAdd=${sumToAdd}`, {
                method: 'PATCH',
            }
        );
    }

    let submitPromise
</script>

<div class="flex flex-col items-center justify-center p-10">
    <Form title="Пополнить счет" submitEventName="signInFormSubmit"
          on:signInFormSubmit={() => submitPromise = handleSubmit()}>
        <svelte:fragment slot="inputs">
            <LabeledFormInput
                    title="Сумма"
                    placeholder="9999"
                    bind:value={sumToAdd}
                    inputType="number"
                    required
            />
        </svelte:fragment>
        <svelte:fragment slot="controls">
            <a class="btn btn-ghost" href="/">Назад</a>
            <button class="btn btn-primary btn-outline" type="submit">Продолжить</button>
            
        </svelte:fragment>
        
    </Form>
    Обновите страницу, чтобы увидеть изменения!
</div>