import { useState } from "react";

export default function CheckoutInput({
  icon: Icon,
  label,
  placeholder,
  textarea = false,
  select = false,
  options = [],
  className = "",
  value,
  onChange,
  name,
}) {
  if (textarea) {
    return (
      <div
        className={`rounded-md border border-[#ECECEC] bg-white px-4 py-2 ${className}`}
      >
        {/* Header */}
        <div className="flex items-center gap-2">
          {Icon && <Icon size={15} className="text-[#00319D]" />}

          <span className="text-[10px] text-[#00319D] font-normal">
            {label}
          </span>
        </div>

        {/* Textarea */}
        <textarea
          name={name}
          value={value ?? ""}
          onChange={onChange}
          rows={2}
          placeholder={placeholder}
          className="mt-1 h-3 w-full mr-5 mx-auto resize-none bg-transparent text-[8px] text-[#00319D] placeholder:text-[#B9B9B9] outline-none"
        />
      </div>
    );
  }

  if (select) {
    return (
      <div
        className={`flex h-10 items-center justify-between rounded-md border border-[#ECECEC] bg-white pr-3 pl-2 ${className}`}
      >
        <div className="flex flex-1 items-center gap-2">
          {Icon && <Icon size={15} className="text-[#00319D] shrink-0" />}

          <select
            name={name}
            value={value ?? ""}
            onChange={onChange}
            className="w-full bg-transparent text-[10px] text-[#00319D] outline-none appearance-none"
          >
            <option value="" disabled>
              {label}
            </option>
            {options.map((option) => (
              <option key={option} value={option}>
                {option}
              </option>
            ))}
          </select>
        </div>

        <svg
          width="15"
          height="18"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          className="text-[#00319D] shrink-0"
        >
          <polyline points="6 9 12 15 18 9" />
        </svg>
      </div>
    );
  }

  return (
    <div
      className={`flex h-10 items-center gap-2 rounded-md border border-[#ECECEC] bg-white px-4 ${className}`}
    >
      {Icon && <Icon size={15} className="text-[#00319D]" />}

      <input
        name={name}
        type="text"
        value={value ?? ""}
        onChange={onChange}
        placeholder={placeholder}
        className="w-full bg-transparent text-[10px] text-[#00319D] placeholder:text-[#00319D] outline-none"
      />
    </div>
  );
}