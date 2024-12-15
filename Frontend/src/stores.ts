import { create } from "zustand";

interface TokenStore {
    token: string | null;
    role: string | null;
    setToken: (token: string, role: string) => void;
    deleteToken: () => void;
}

export const useTokenStore = create<TokenStore>((set) => ({
    token: sessionStorage.getItem("token") || null,
    role: sessionStorage.getItem("role") || null,
    setToken: (token: string, role: string | null) => {
        set({ token: token, role: role });
        sessionStorage.setItem("token", token);
        if (role != null) {
            sessionStorage.setItem("role", role);
        }
    },
    deleteToken: () => {
        set({ token: null, role: null });
        sessionStorage.removeItem("token");
        sessionStorage.removeItem("role");
    }
}));