export async function authenticateUser(fetchDelegate: (input: (URL | RequestInfo), init?: (RequestInit | undefined)) => Promise<Response>): Promise<AuthData> {
    const response = await fetchDelegate(`/api/auth/get-permissions`);
    if (response.ok) {
        let body = await response.json()
        return body
    } else if (response.status === 401 || response.status === 403) {
        return null!
    } else {
        throw new Error('Failed to fetch /api/auth/check-authorized');
    }
}

interface AuthData {
    systemName: string,
    title: string,
    description: string
}