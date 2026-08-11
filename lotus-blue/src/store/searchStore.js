import { create } from "zustand";

const RECENT_KEY = "recent-searches";

const getRecent = () => {
  try {
    return JSON.parse(localStorage.getItem(RECENT_KEY)) || [];
  } catch {
    return [];
  }
};

export const useSearchStore = create((set, get) => ({
  query: "",

  recentSearches: getRecent(),

  setQuery: (query) => set({ query }),

  addRecentSearch: (value) => {
    if (!value.trim()) return;

    const recent = [
      value,
      ...get().recentSearches.filter((i) => i !== value),
    ].slice(0, 8);

    localStorage.setItem(
      RECENT_KEY,
      JSON.stringify(recent)
    );

    set({
      recentSearches: recent,
    });
  },

  removeRecentSearch: (value) => {
    const recent = get().recentSearches.filter(
      (i) => i !== value
    );

    localStorage.setItem(
      RECENT_KEY,
      JSON.stringify(recent)
    );

    set({
      recentSearches: recent,
    });
  },

  clearRecentSearches: () => {
    localStorage.removeItem(RECENT_KEY);

    set({
      recentSearches: [],
    });
  },
}));