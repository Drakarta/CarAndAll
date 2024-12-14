import { create } from "zustand";

interface TokenStore {
    token: string | null;
    setToken: (token: string) => void;
    deleteToken: () => void;
}

export const useTokenStore = create<TokenStore>((set) => ({
    token: localStorage.getItem("token") || null,
    setToken: (token: string) => {
        set({token})
        window.sessionStorage.setItem("token", token)
    },
    deleteToken: () => {
        set({token: null})
        window.sessionStorage.removeItem("token")
    }
}));