import DividerFlower from "./../../components/common/DividerFlower";
import { useState } from "react";
import {
  ChevronDown,
  ChevronUp,
  Search,
  Minus,
  MessageCircle,
} from "lucide-react";

const faqs = [
  {
    question: "كيف أختار درجة الفاونديشن المناسبة؟",
    answer:
      "يمكنك اختيار الدرجة المناسبة من خلال تحديد لون بشرتك (فاتح، متوسط، داكن) ثم اختيار النغمة (دافئة، محايدة، باردة). كما نوفر أداة مطابقة الدرجات لمساعدتك في اختيار الدرجة الأنسب لك بسهولة.",
  },
  {
    question: "ما الفرق بين التغطية المتوسطة والكاملة؟",
    answer:
      "التغطية المتوسطة تمنحك مظهراً طبيعياً مع توحيد لون البشرة وإخفاء العيوب الخفيفة، بينما التغطية الكاملة تخفي العيوب بشكل أكبر وتمنحك مظهراً أكثر تغطية وثباتاً.",
  },
  {
    question: "هل المنتجات أصلية؟",
    answer:
      "نعم، جميع المنتجات المتوفرة لدينا أصلية ومختارة بعناية من مصادر موثوقة لضمان أفضل جودة.",
  },
  {
    question: "هل يمكن تبديل درجة اللون؟",
    answer:
      "نعم، يمكنك طلب تبديل درجة اللون وفقاً لسياسة الاستبدال الخاصة بالمنتج، بشرط أن يكون المنتج بحالته الأصلية.",
  },
  {
    question: "كم مدة التوصيل؟",
    answer:
      "عادةً يستغرق التوصيل من 2 إلى 5 أيام عمل حسب موقعك وطريقة الشحن المختارة.",
  },
  {
    question: "هل الدفع عند الاستلام متوفر؟",
    answer: "نعم، الدفع عند الاستلام متوفر في المناطق التي تدعم هذه الخدمة.",
  },
  {
    question: "كيف أعرف المنتج المناسب لنوع بشرتي؟",
    answer:
      "يمكنك معرفة المنتجات المناسبة لك من خلال قسم نوع البشرة واختيار نوع بشرتك للحصول على المنتجات المقترحة.",
  },
  {
    question: "كيف أحافظ على المنتج بعد فتحه؟",
    answer:
      "احفظي المنتجات في مكان جاف وبعيد عن أشعة الشمس المباشرة والحرارة، واحرصي على إغلاق العبوة جيداً بعد كل استخدام.",
  },
];

export default function FAQ() {
  const [openIndex, setOpenIndex] = useState(0);
  const [search, setSearch] = useState("");

  const filteredFaqs = faqs.filter((faq) =>
    faq.question.toLowerCase().includes(search.toLowerCase()),
  );

  return (
    <main dir="rtl" className="min-h-screen px-4">
      {/* ================= HEADER ================= */}

      <header className="mt-3 text-center">
        <h1 className="text-[17px] font-semibold text-[#0B2E74]">
          الأسئلة الشائعة
        </h1>

        {/* Flower Divider */}
        <div className="mx-auto  w-[100px] text-[#F2A8AE]">
          <DividerFlower />
        </div>

        <p className="text-[10px] text-[#777777]">
          إجابات سريعة على أكثر الأسئلة شيوعاً
        </p>
      </header>

      {/* ================= SEARCH ================= */}

      <div className="relative mt-2">
        <Search
          size={17}
          strokeWidth={1.5}
          className="absolute left-4 top-1/2 -translate-y-1/2 text-[#0B2E74]"
        />

        <input
          type="text"
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setOpenIndex(null);
          }}
          placeholder="إبحثي عن سؤالك"
          className="
            h-[38px]
            w-full
            rounded-[14px]
            border
            border-[#F0C9CD]
            bg-white
            px-4
            pl-12
            text-right
            text-[12px]
            text-[#3c3c3c]
            outline-none
            placeholder:text-[#A5A5A5]
            focus:border-[#DFA0A7]
          "
        />
      </div>

      {/* ================= FAQ LIST ================= */}

      <section className="mt-3 space-y-2">
        {filteredFaqs.map((faq, index) => {
          const isOpen = openIndex === index;

          return (
            <div
              key={faq.question}
              className={`
                overflow-hidden
                rounded-[14px]
                border
                transition-all
                ${
                  isOpen
                    ? "border-[#F1E0E1] bg-[#FFF8F8]"
                    : "border-[#EEEEEE] bg-white"
                }
              `}
            >
              {/* Question */}

              <button
                dir="ltr"
                type="button"
                onClick={() => setOpenIndex(isOpen ? null : index)}
                className="flex min-h-[48px] w-full items-center justify-between gap-3 px-4 text-right"
              >
                {/* Arrow */}

                {isOpen ? (
                  <ChevronUp
                    size={20}
                    strokeWidth={1.7}
                    className="shrink-0 text-[#00319D]"
                  />
                ) : (
                  <ChevronDown
                    size={20}
                    strokeWidth={1.7}
                    className="shrink-0 text-[#00319D]"
                  />
                )}

                <span
                  className={`
                    flex-1
                    text-[12px]
                    font-medium
                    ${isOpen ? "text-[#0B2E74]" : "text-[#00319D]"}
                  `}
                >
                  {faq.question}
                </span>

                {/* Open indicator */}

                {isOpen && (
                  <span className="flex h-[18px] w-[18px] shrink-0 items-center justify-center rounded-full bg-[#F3B9BE]">
                    <Minus size={11} strokeWidth={2} className="text-white" />
                  </span>
                )}
              </button>

              {/* Answer */}

              {isOpen && (
                <div className="px-10 pb-2">
                  <p className="text-right text-[10px] leading-4 text-[#575757]">
                    {faq.answer}
                  </p>
                </div>
              )}
            </div>
          );
        })}
      </section>

      {/* ================= NO RESULTS ================= */}

      {filteredFaqs.length === 0 && (
        <div className="py-10 text-center">
          <p className="text-[12px] text-[#888]">لم نجد سؤالاً مطابقاً لبحثك</p>
        </div>
      )}

      {/* ================= CONTACT CARD ================= */}

      {/* Contact CTA */}
      <div className="relative mt-5 h-[100px]  w-full overflow-hidden rounded-[8px] bg-[#FFF9F9]">
        {/* Left lotus */}
        <div className="absolute -bottom-5 -left-2 h-[105px] w-[145px] opacity-45">
          <svg
            viewBox="0 0 150 110"
            className="h-full w-full"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <g
              stroke="#F6C8CC"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M12 103C28 78 43 61 63 51C55 70 51 87 50 104" />
              <path d="M12 103C26 86 26 67 17 49C36 57 47 71 50 92" />
              <path d="M50 103C53 79 66 57 85 40C82 64 75 87 63 105" />
              <path d="M50 103C43 78 47 57 62 34C69 60 65 84 57 104" />
              <path d="M50 103C68 87 87 80 108 80C92 96 74 102 55 105" />
              <path d="M50 103C73 101 95 106 117 109" />

              <path d="M12 103C38 105 64 106 91 109" />
            </g>
          </svg>
        </div>

        {/* Right lotus */}
        <div className="absolute -bottom-4 -right-3 h-[115px] w-[150px] opacity-45">
          <svg
            viewBox="0 0 150 110"
            className="h-full w-full"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <g
              stroke="#F6C8CC"
              strokeWidth="1.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M138 103C122 78 107 61 87 51C95 70 99 87 100 104" />
              <path d="M138 103C124 86 124 67 133 49C114 57 103 71 100 92" />
              <path d="M100 103C97 79 84 57 65 40C68 64 75 87 87 105" />
              <path d="M100 103C107 78 103 57 88 34C81 60 85 84 93 104" />
              <path d="M100 103C82 87 63 80 42 80C58 96 76 102 95 105" />
              <path d="M100 103C77 101 55 106 33 109" />

              <path d="M138 103C112 105 86 106 59 109" />
            </g>
          </svg>
        </div>

        {/* Content */}
        <div
          dir="rtl"
          className="relative z-10 flex h-full items-center justify-center gap-7 px-5"
        >
          {/* Chat icon */}
          {/* Text + Button */}
          <div className="flex min-w-0 flex-col flex-1 items-center">
            <h3 className="text-[14px] font-medium text-[#0B2E74] md:text-[23px]">
              لا تجدين إجابتك؟
            </h3>

            <p className="mt-1 text-[9px] text-center leading-4 text-[#777777] md:text-[15px]">
              فريقنا جاهز لمساعدتك
              <br />
              نحن هنا لخدمتك بكل حب
            </p>

            <button
              type="button"
              className="mt-2 flex h-[25px] min-w-[100px] items-center justify-center rounded-[6px] bg-[#0B2E74]  text-[11px] text-white transition hover:bg-[#08265F] md:h-[44px] md:min-w-[175px] md:text-[16px]"
            >
              تواصلي معنا
            </button>
          </div>
          <div className="relative flex h-[80px] w-[80px]  shrink-0 items-center justify-center -z-20">
            {/* Pink circle */}
            <div className="absolute inset-0 rounded-full bg-[#FDEBED] " />

            {/* Bubble */}
            <svg
              viewBox="0 -5 100 90"
              className="relative z-0 h-[68px] w-[68px]"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              {/* Bubble body */}
              <path
                d="
                    M50 10
                    C28 10 14 22 14 40
                    C14 58 28 70 50 70
                    C72 70 86 58 86 40
                    C86 22 72 10 50 10
                    Z

                    M31 50
                    L22 80
                    L42 70
                    Z
                "
                fill="#0B2E74"
              />

              {/* Dots */}
              <circle cx="35" cy="40" r="5" fill="white" />
              <circle cx="51" cy="40" r="5" fill="white" />
              <circle cx="67" cy="40" r="5" fill="white" />
            </svg>
          </div>
        </div>
      </div>
    </main>
  );
}
