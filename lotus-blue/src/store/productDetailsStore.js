import { create } from "zustand";

export const useProductDetailsStore = create((set) => ({
  selectedShade: 0,
  quantity: 1,

  setShade: (index) =>
    set({
      selectedShade: index,
    }),

  increase: () =>
    set((state) => ({
      quantity: state.quantity + 1,
    })),

  decrease: () =>
    set((state) => ({
      quantity: Math.max(1, state.quantity - 1),
    })),

  reset: () =>
    set({
      selectedShade: 0,
      quantity: 1,
    }),
}));