export async function signIn(login: string, password: string): Promise<void | string[]> {
    const response = await fetch('/api/auth/sign-in', {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({login, password}),
    });

    if (response.ok) {
        return []
    } else if (response.status === 404) {
        const errorData = await response.json();
        return errorData.errors;
    } else {
        throw new Error(response.status.toString())
    }
};

export async function signOut() {
    const response = await fetch('/api/auth/sign-out', {method: 'POST'});
}