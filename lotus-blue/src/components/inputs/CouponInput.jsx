import { Tag } from "lucide-react";
import { useState } from "react";
import Button from "../buttons/Button.jsx";

export default function CouponInput({ onApply }) {
  const [code, setCode] = useState("");

  return (
    <div className="flex items-center gap-2 rounded-md border border-gray-300 pr-3">
      <Tag
        size={18}
        className="shrink-0 rotate-[90deg] text-gray-navy opacity-75"
      />

      <input
        value={code}
        onChange={(e) => setCode(e.target.value)}
        type="text"
        placeholder="كود الخصم"
        className="
          min-w-0
          flex-1
          bg-transparent
          py-1
          text-right
          text-sm
          text-[#00319D]
          outline-none
          placeholder:text-gray-500
        "
      />

      <Button
        className="
          shrink-0
          !border-0
          !bg-[#00319D]/10
          !px-3
          !py-1.5
          !text-[#00319D]
          !font-light
        "
        onClick={() => onApply?.(code)}
      >
        تطبيق
      </Button>
    </div>
  );
}