import { create } from "zustand";
import { apiGet, apiPut } from "../lib/apiClient.js";

export const useAccountStore = create((set) => ({
  profile: null,
  isLoading: false,
  error: null,

  fetchProfile: async () => {
    set({ isLoading: true, error: null });
    try {
      const profile = await apiGet("/api/customers/me");
      set({ profile, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },

  updateProfile: async (data) => {
    const profile = await apiPut("/api/customers/me", data);
    set({ profile });
    return profile;
  },
}));
