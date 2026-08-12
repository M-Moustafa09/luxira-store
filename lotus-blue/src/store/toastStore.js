import { create } from "zustand";

let nextId = 1;

export const useToastStore = create((set, get) => ({
  toasts: [],

  showToast: (message, type = "success") => {
    const id = nextId++;
    set({ toasts: [...get().toasts, { id, message, type }] });

    setTimeout(() => {
      set({ toasts: get().toasts.filter((t) => t.id !== id) });
    }, 2500);
  },

  dismissToast: (id) => {
    set({ toasts: get().toasts.filter((t) => t.id !== id) });
  },
}));
