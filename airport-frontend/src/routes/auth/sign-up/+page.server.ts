import type { Actions } from './$types';

export const actions = {
    default: async ({cookies, request}) => {
        let data = await request.formData()
        
    },
} satisfies Actions;