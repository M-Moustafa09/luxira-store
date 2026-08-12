import { create } from "zustand";
import { apiPost } from "../lib/apiClient.js";
import {
  getAccessToken,
  getRefreshToken,
  setTokens,
  clearTokens,
} from "../lib/authToken.js";

export const useAuthStore = create((set) => ({
  customer: null,
  isAuthenticated: !!getAccessToken(),

  register: async ({ name, email, phone, password }) => {
    const response = await apiPost("/api/auth/register", { name, email, phone, password });
    setTokens(response);
    set({ customer: response.customer, isAuthenticated: true });
  },

  login: async ({ email, password }) => {
    const response = await apiPost("/api/auth/login", { email, password });
    setTokens(response);
    set({ customer: response.customer, isAuthenticated: true });
  },

  // Only clears the JWT - deliberately leaves the X-Guest-Id untouched.
  // After register (Option B), CustomerId IS the guest id, so wiping it
  // here would orphan the account's own cart, the exact bug this whole
  // feature was built to fix.
  logout: async () => {
    const refreshToken = getRefreshToken();
    clearTokens();
    set({ customer: null, isAuthenticated: false });

    if (refreshToken) {
      try {
        await apiPost("/api/auth/logout", { refreshToken });
      } catch {
        // Already invalid/expired - nothing left to revoke server-side.
      }
    }
  },

  // Called once on app start: turns a stored refresh token back into a
  // live access token, so a returning customer resolves via their JWT
  // instead of silently falling back to the X-Guest-Id guest identity.
  hydrate: async () => {
    const refreshToken = getRefreshToken();
    if (!refreshToken) return;

    try {
      const response = await apiPost("/api/auth/refresh", { refreshToken });
      setTokens(response);
      set({ customer: response.customer, isAuthenticated: true });
    } catch {
      clearTokens();
      set({ customer: null, isAuthenticated: false });
    }
  },
}));
