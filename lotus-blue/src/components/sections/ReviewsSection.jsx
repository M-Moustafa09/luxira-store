import { useEffect, useState } from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import "swiper/css";

import { useReviewsStore } from "../../store/reviewsStore.js";
import ReviewCard from "../cards/ReviewCard.jsx";

export default function ReviewsSection() {
  const [active, setActive] = useState(0);
  const reviews = useReviewsStore((s) => s.reviews);
  const fetchReviews = useReviewsStore((s) => s.fetchReviews);

  useEffect(() => {
    fetchReviews();
  }, [fetchReviews]);

  if (reviews.length === 0) return null;

  return (
    <div className="px-2 lg:px-0">
      <Swiper
        slidesPerView={1}
        spaceBetween={12}
        loop
        onSlideChange={(swiper) => setActive(swiper.realIndex)}
      >
        {reviews.map((review) => (
          <SwiperSlide key={review.id}>
            <ReviewCard
              review={review}
              active={active}
              total={reviews.length}
            />
          </SwiperSlide>
        ))}
      </Swiper>
    </div>
  );
}