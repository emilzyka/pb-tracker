const BASE_URL = import.meta.env.VITE_FA_BASE_URL;
const FA_API_KEY = import.meta.env.VITE_FA_API_KEY;

export async function loginUser(username: string, pwd: string): Promise<void> {
    console.log(`${BASE_URL}/Login${FA_API_KEY}`);
    const response = await fetch(`${BASE_URL}/Login${FA_API_KEY}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        credentials: 'include',
        body: JSON.stringify({ username, pwd }),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.message || 'Login failed');
    }

    const data = await response.json();

    // Save user-id to local storage
    localStorage.setItem('userId', data.id);

    return;
}

export async function registerUser(
    username: string,
    pwd: string
): Promise<void> {
    const response = await fetch(`${BASE_URL}/Register${FA_API_KEY}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ username, pwd }),
    });

    if (!response.ok) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.message || 'Signup failed');
    }
}
