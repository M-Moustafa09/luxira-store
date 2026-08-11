import { create } from 'zustand'

export const useFiltersStore = create((set) => ({
  sort: 'default',
  skinType: null,
  brand: null,
  priceRange: null,
  category: null,
  setSort: (sort) => set({ sort }),
  setCategory: (category) => set({ category }),
  reset: () => set({ sort: 'default', skinType: null, brand: null, priceRange: null, category: null }),
}))
