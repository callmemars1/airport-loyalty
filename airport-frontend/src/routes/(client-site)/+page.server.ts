import type {PageServerLoad} from './$types';
import {authenticateUser} from '$lib/server/auth';

export const load: PageServerLoad = async ({fetch, params}) => {
    const role = await authenticateUser(fetch)
    return {
        roleSystemName: role?.systemName
    }
};