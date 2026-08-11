import { useProductDetailsStore } from "../../store/productDetailsStore";

export default function ShadeSelector({ product }) {
  const selectedShade = useProductDetailsStore((s) => s.selectedShade);
  const setShade = useProductDetailsStore((s) => s.setShade);

  return (
    <div>
      {/* Shades */}

      <div className="flex items-center justify-between">
        {product.variants.map((variant, index) => (
          <button
            key={variant.id}
            onClick={() => setShade(index)}
            className={`flex h-5 w-5 items-center justify-center rounded-full transition ${
              selectedShade === index
                ? "border-[1px] border-[#1E2F5F]"
                : "border border-transparent"
            }`}
          >
            <span
              style={{ backgroundColor: variant.colorHex }}
              className="h-4 w-4 rounded-full"
            />
          </button>
        ))}
      </div>

      {/* Line */}

      <div className="mt-3">
        <div className="relative flex items-center">
          <span className="h-[4px] w-[4px] rounded-full bg-[#c9c9c9]" />

          <div className="h-[1px] flex-1 bg-[#c9c9c9]" />

          <span className="h-[4px] w-[5px] rounded-full bg-[#c9c9c9]" />
        </div>

        <div className="px-1 mt-1 flex justify-between text-[7px] text-[#535353]">
          <span>فاتح</span>
          <span>متوسط</span>
          <span>داكن</span>
        </div>
      </div>
    </div>
  );
}
