import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

export const useReviewsStore = create((set, get) => ({
  reviews: [],
  isLoading: false,
  error: null,

  fetchReviews: async () => {
    if (get().isLoading || get().reviews.length > 0) return;

    set({ isLoading: true, error: null });
    try {
      const reviews = await apiGet("/api/testimonials");
      set({ reviews, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },
}));
