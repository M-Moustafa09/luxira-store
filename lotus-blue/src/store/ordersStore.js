import { create } from "zustand";
import { apiGet } from "../lib/apiClient.js";

export const useOrdersStore = create((set) => ({
  orders: [],
  isLoading: false,
  error: null,

  fetchMyOrders: async () => {
    set({ isLoading: true, error: null });
    try {
      const result = await apiGet("/api/orders/mine?pageSize=20");
      set({ orders: result.items, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },
}));
