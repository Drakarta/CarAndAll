import { create } from "zustand";

interface TokenStore {
    token: string | null;
    setToken: (token: string) => void;
    deleteToken: () => void;
}

export const useTokenStore = create<TokenStore>((set) => ({
    token: localStorage.getItem("token") || null,
    setToken: (token: string) => set({token}),
    deleteToken: () => {
        set({token: null})
        localStorage.removeItem("token")
    }
}));