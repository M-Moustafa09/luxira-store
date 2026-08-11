import { create } from "zustand";
import { apiGet, apiPost, apiPut, apiDelete } from "../lib/apiClient.js";

export const useAddressesStore = create((set) => ({
  addresses: [],
  isLoading: false,
  error: null,

  fetchAddresses: async () => {
    set({ isLoading: true, error: null });
    try {
      const addresses = await apiGet("/api/addresses");
      set({ addresses, isLoading: false });
    } catch (error) {
      set({ error: error.message, isLoading: false });
    }
  },

  createAddress: async (data) => {
    const addresses = await apiPost("/api/addresses", data).then(
      () => apiGet("/api/addresses"),
    );
    set({ addresses });
  },

  updateAddress: async (id, data) => {
    await apiPut(`/api/addresses/${id}`, data);
    const addresses = await apiGet("/api/addresses");
    set({ addresses });
  },

  deleteAddress: async (id) => {
    await apiDelete(`/api/addresses/${id}`);
    const addresses = await apiGet("/api/addresses");
    set({ addresses });
  },
}));
