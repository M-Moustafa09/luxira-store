export default function SkinProgress({ currentStep = 1 }) {
  const steps = [
    "لون البشرة",
    "الأندرتون",
    "تحديد الدرجة",
    "النتيجة",
  ];

  return (
    <div className="mx-auto w-[95%] mt-3">
      {/* Circles + connecting lines */}
      <div className="flex w-full items-center">
        {steps.map((step, index) => {
          const number = index + 1;
          const active = number === currentStep;
          const completed = number < currentStep;

          return (
            <div
              key={step}
              className="flex flex-1 items-center last:flex-none"
            >
              {/* Step */}
              <div className="flex h-7 w-7 shrink-0 items-center justify-center">
                <div
                  className={`flex items-center justify-center rounded-full border text-[11px] font-medium transition-all ${
                    active
                      ? "h-7 w-7 border-[#F08B94] bg-[#F08B94] text-white"
                      : completed
                      ? "h-6 w-6 border-[#F08B94] bg-[#F08B94] text-white"
                      : "h-6 w-6 border-[#D8D8D8] bg-white text-[#B0B0B0]"
                  }`}
                >
                  {number}
                </div>
              </div>

              {/* Line */}
              {index !== steps.length - 1 && (
                <div className="relative h-[3px] flex-1 overflow-hidden rounded-full bg-[#E5E5E5]">
                  {/* Completed line */}
                  {completed && (
                    <div className="absolute inset-0 bg-[#F08B94]" />
                  )}

                  {/* Current line: 75% pink */}
                  {active && (
                    <div className="absolute left-0 top-0 h-full w-[75%] bg-[#F08B94]" />
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Labels */}
      <div className="mt-2 flex">
        {steps.map((step, index) => {
          const number = index + 1;
          const active = number === currentStep;

          return (
            <span
              key={step}
              className={`flex-1 whitespace-nowrap text-center text-[9px] md:text-[13px] ${
                active
                  ? "font-medium text-[#F08B94]"
                  : "text-[#B0B0B0]"
              }`}
            >
              {step}
            </span>
          );
        })}
      </div>
    </div>
  );
}