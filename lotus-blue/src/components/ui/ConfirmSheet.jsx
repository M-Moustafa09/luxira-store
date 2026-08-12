export default function ConfirmSheet({
  open,
  message,
  confirmLabel = "حذف",
  cancelLabel = "إلغاء",
  onConfirm,
  onCancel,
}) {
  if (!open) return null;

  return (
    <>
      {/* Overlay */}
      <div onClick={onCancel} className="fixed inset-0 z-40 bg-black/30" />

      {/* Sheet */}
      <div className="fixed bottom-0 left-0 right-0 z-50 rounded-t-[28px] bg-white px-5 pb-8 pt-4">
        {/* Handle */}
        <div className="mx-auto h-1 w-12 rounded-full bg-[#EAEAEA]" />

        <p className="mt-6 text-center text-[14px] text-[#00319D]">
          {message}
        </p>

        <div className="mt-6 flex gap-3">
          <button
            type="button"
            onClick={onCancel}
            className="
              flex-1
              rounded-md
              border
              border-[#00319D]
              bg-transparent
              py-2.5
              text-sm
              font-semibold
              text-[#00319D]
              transition
              active:scale-[0.98]
            "
          >
            {cancelLabel}
          </button>

          <button
            type="button"
            onClick={onConfirm}
            className="
              flex-1
              rounded-md
              bg-red-500
              py-2.5
              text-sm
              font-semibold
              text-white
              transition
              hover:bg-red-600
              active:scale-[0.98]
            "
          >
            {confirmLabel}
          </button>
        </div>
      </div>
    </>
  );
}
