import { AnimatePresence, motion } from "framer-motion";
import { CheckCircle2, XCircle } from "lucide-react";
import { useToastStore } from "../../store/toastStore.js";

export default function ToastContainer() {
  const toasts = useToastStore((s) => s.toasts);

  return (
    <div className="fixed top-4 inset-x-0 z-[200] flex flex-col items-center gap-2 pointer-events-none px-4">
      <AnimatePresence>
        {toasts.map((toast) => (
          <motion.div
            key={toast.id}
            initial={{ opacity: 0, y: -16 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -16 }}
            transition={{ duration: 0.2 }}
            className="flex items-center gap-2 bg-navy text-white text-sm font-body px-4 py-2.5 rounded-xl2 shadow-card max-w-sm"
          >
            {toast.type === "success" ? (
              <CheckCircle2 size={18} className="text-blush-400 shrink-0" />
            ) : (
              <XCircle size={18} className="text-blush-400 shrink-0" />
            )}
            <span>{toast.message}</span>
          </motion.div>
        ))}
      </AnimatePresence>
    </div>
  );
}
