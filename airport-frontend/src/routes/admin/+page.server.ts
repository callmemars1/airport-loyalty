import type {PageServerLoad} from './$types';
import {authenticateUser} from '$lib/server/auth';
import { redirect } from "@sveltejs/kit"

export const load: PageServerLoad = async ({fetch, params}) => {
    const role = await authenticateUser(fetch)
    console.log(role)
    if (!role || !['Admin', 'Editor', 'Analyst'].find(r => role.systemName === r)){
        throw redirect(302, '/auth/sign-in')
    }
    
    return {
        roleSystemName: role.systemName
    }
};