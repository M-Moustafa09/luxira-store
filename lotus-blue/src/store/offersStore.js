import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

export const useOffersStore = create((set, get) => ({
  bundles: [],
  buyMoreOffers: [],
  campaign: null,
  isLoading: false,
  error: null,

  fetchOffers: async () => {
    if (get().isLoading) return;

    set({ isLoading: true, error: null });
    try {
      const [bundles, buyMoreOffers, campaign] = await Promise.all([
        apiGet("/api/bundles"),
        apiGet("/api/promotions/buy-more-offers"),
        apiGet("/api/promotions/campaign"),
      ]);

      set({ bundles, buyMoreOffers, campaign, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },
}));
