import { useEffect, useState } from "react";
import { Star, Store } from "lucide-react";

import { apiGet, apiPost } from "../../lib/apiClient.js";
import { useToastStore } from "../../store/toastStore.js";
import { usePolling } from "../../hooks/usePolling.js";
import SectionTitle from "../sections/SectionTitle.jsx";
import CheckoutInput from "../checkout/CheckoutInput.jsx";
import Button from "../buttons/Button.jsx";

function StarPicker({ value, onChange }) {
  return (
    <div dir="ltr" className="flex gap-1">
      {Array.from({ length: 5 }).map((_, i) => {
        const starValue = i + 1;
        return (
          <button
            key={i}
            type="button"
            onClick={() => onChange(starValue)}
            aria-label={`${starValue} نجوم`}
          >
            <Star
              size={22}
              className={
                starValue <= value
                  ? "fill-amber-400 text-amber-400"
                  : "fill-gray-200 text-gray-200"
              }
            />
          </button>
        );
      })}
    </div>
  );
}

// Any visitor (guest or registered) can post a review - the backend resolves
// identity the same way as every other endpoint (guest-id/JWT), this form
// just always asks for a display name since a guest has none on file.
export default function ProductReviews({ productId, onReviewAdded }) {
  const [reviews, setReviews] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [authorName, setAuthorName] = useState("");
  const [rating, setRating] = useState(0);
  const [text, setText] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const showToast = useToastStore((s) => s.showToast);

  const fetchReviews = () => {
    apiGet(`/api/products/${productId}/reviews`)
      .then((result) => setReviews(result.items))
      .catch(() => {})
      .finally(() => setIsLoading(false));
  };

  useEffect(() => {
    setIsLoading(true);
    fetchReviews();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [productId]);

  // The auto-reply (Module B) lands 1-5 minutes after a review is submitted,
  // via a backend job - polling so it appears on an already-open product
  // page without the customer needing to refresh. Same interval as the
  // Offers banner/"my orders" list (not as time-sensitive as live order
  // tracking, so no need for TrackOrder's faster 18s).
  usePolling(fetchReviews, 25000);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!authorName.trim() || !text.trim() || rating === 0) {
      setError("من فضلك أدخلي اسمك، تقييمك، وتعليقك");
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      await apiPost(`/api/products/${productId}/reviews`, {
        authorName,
        rating,
        text,
      });

      setAuthorName("");
      setRating(0);
      setText("");
      showToast("تم إضافة تقييمك بنجاح");
      fetchReviews();
      onReviewAdded?.();
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <section className="mt-3 sm:mt-4 md:mt-5">
      <SectionTitle title="آراء العملاء" />

      <form
        onSubmit={handleSubmit}
        className="mx-4 lg:mx-0 flex flex-col gap-2 rounded-md border border-[#ECECEC] bg-white p-4"
      >
        <span className="text-[11px] font-semibold text-[#00319D]">
          أضيفي رأيك
        </span>

        <StarPicker value={rating} onChange={setRating} />

        <CheckoutInput
          placeholder="اسمك"
          value={authorName}
          onChange={(e) => setAuthorName(e.target.value)}
        />

        <CheckoutInput
          textarea
          label="تعليقك"
          placeholder="اكتبي رأيك في المنتج"
          value={text}
          onChange={(e) => setText(e.target.value)}
        />

        {error && <p className="text-[10px] text-red-500">{error}</p>}

        <Button type="submit" disabled={isSubmitting} className="h-9">
          {isSubmitting ? "جارٍ الإرسال..." : "إرسال التقييم"}
        </Button>
      </form>

      <div className="mx-4 lg:mx-0 mt-3 flex flex-col gap-2">
        {!isLoading && reviews.length === 0 && (
          <p className="text-center text-[11px] text-[#8F97AE]">
            لا توجد تقييمات بعد، كوني أول من يقيّم هذا المنتج
          </p>
        )}

        {reviews.map((review) => (
          <div
            key={review.id}
            className="rounded-md border border-[#ECECEC] bg-white p-3"
          >
            <div className="flex items-center justify-between">
              <span className="text-[11px] font-semibold text-[#00319D]">
                {review.authorName}
              </span>

              <div dir="ltr" className="flex gap-0.5">
                {Array.from({ length: 5 }).map((_, i) => (
                  <Star
                    key={i}
                    size={12}
                    className={
                      i < review.rating
                        ? "fill-amber-400 text-amber-400"
                        : "fill-gray-300 text-gray-300"
                    }
                  />
                ))}
              </div>
            </div>

            <p className="mt-1 text-[10px] text-[#666666]">{review.text}</p>

            {review.replies?.length > 0 && (
              <div className="mt-2 flex flex-col gap-2 border-t border-[#ECECEC] pt-2">
                {review.replies.map((reply) => (
                  <div
                    key={reply.id}
                    className="rounded-md bg-[#F5F7FF] p-2"
                  >
                    <div className="flex items-center gap-1">
                      {reply.isAutomated && (
                        <span className="flex items-center gap-1 rounded-full bg-[#00319D] px-2 py-0.5 text-[9px] font-semibold text-white">
                          <Store size={10} />
                          رد فريق المتجر
                        </span>
                      )}
                    </div>
                    <p className="mt-1 text-[10px] text-[#444444]">
                      {reply.text}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>
    </section>
  );
}
