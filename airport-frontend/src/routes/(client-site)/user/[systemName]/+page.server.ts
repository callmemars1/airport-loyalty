import type {PageServerLoad} from './$types';
import {authenticateUser} from '$lib/server/auth';
import { redirect, error } from "@sveltejs/kit"

export const load: PageServerLoad = async ({fetch, params}) => {
    const role = await authenticateUser(fetch)
    console.log(role)
    if (!role) {
        throw redirect(302, '/auth/sign-in')
    }
    
    let response = await fetch('/api/users/current-data')
    let data = await response.json()
    
    if (data.login !== params.systemName){
        console.log('some')
        throw error(404, {
            message: "Страница не найдена"
        })
    }
    
    return {
        roleSystemName: role?.systemName
    }
};