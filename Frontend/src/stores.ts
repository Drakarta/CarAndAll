import { create } from "zustand";

interface TokenStore {
    token: string | null;
    setToken: (token: string) => void;
    deleteToken: () => void;
}

export const useTokenStore = create<TokenStore>((set) => ({
    token: sessionStorage.getItem("token") || null,
    setToken: (token: string) => {
        set({ token });
        sessionStorage.setItem("token", token);
    },
    deleteToken: () => {
        set({ token: null });
        sessionStorage.removeItem("token");
    }
}));
