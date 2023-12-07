import {writable} from 'svelte/store';


export const isAuthenticated = writable("false");

export async function signIn(login: string, password: string): Promise<void | string[]> {
    const response = await fetch('/api/auth/sign-in', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({login, password}),
    });

    if (response.ok) {
        isAuthenticated.set(true)
        return []
    } else if (response.status === 404) {
        const errorData = await response.json();
        return errorData.errors;
    } else {
        throw new Error(response.status.toString())
    }
};

export function signOut() {
    isAuthenticated.set(false);
}

export async function checkAuthorized(): Promise<void> {
    const response = await fetch(`/api/auth/check-authorized`);
    if (response.ok) {
        isAuthenticated.set(true)
    }
    else if (response.status === 401 || response.status === 403){
        isAuthenticated.set(false)
    }
    else {
        throw new Error('Failed to fetch /api/auth/check-authorized');
    }
}