import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  setToken: (t: string) => void;
  logout: () => void;
  getUserId: () => string | null;
}

function decodeJwtSub(token: string): string | null {
  try {
    const payload = token.split('.')[1];
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    const obj = JSON.parse(json);
    return obj?.sub ?? null;
  } catch {
    return null;
  }
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      setToken: (t) => set({ token: t }),
      logout: () => set({ token: null }),
      getUserId: () => (get().token ? decodeJwtSub(get().token!) : null),
    }),
    { name: 'tasky-auth' }
  )
);
