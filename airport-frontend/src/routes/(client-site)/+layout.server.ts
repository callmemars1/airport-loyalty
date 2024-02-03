import type {LayoutServerLoad} from './$types';
import {authenticateUser} from '$lib/server/auth';

export const load: LayoutServerLoad = async ({fetch, params}) => {
    const role = await authenticateUser(fetch)

    return {
        roleSystemName: role?.systemName
    }
};