import Button from "../buttons/Button.jsx";

export default function PromoBanner({
  eyebrow,
  title,
  subtitle,
  cta = "تسوقي الآن",
  image,
}) {
  return (
    <div
      className="
        mx-4 my-1 lg:mx-0
        rounded-md
        overflow-hidden
        bg-navy-100
        flex items-center
        relative
        h-[clamp(72px,20vw,160px)]
      "
    >
      <div className="flex-1 min-w-0 px-[clamp(10px,3vw,24px)]">
        {eyebrow && (
          <p className="text-[clamp(10px,2.2vw,18px)] font-bold text-blush-500 mb-1 truncate">
            {eyebrow}
          </p>
        )}
        <h3 className="font-bold text-[#00319D] text-[clamp(13px,2.8vw,20px)] mb-1 truncate">
          {title}
        </h3>
        {subtitle && (
          <p className="text-[clamp(11px,2vw,20px)] text-gray-500 mb-2 hidden lg:block">
            {subtitle}
          </p>
        )}
        <Button
          className="!py-1.5 !px-3 text-[clamp(10px,2vw,14px)]"
          to="/new-arrivals"
        >
          {cta}
        </Button>
      </div>
      <div className="absolute bottom-0 left-0 w-[clamp(110px,30vw,320px)]">
        <img
          src={image}
          alt={title}
          className="h-full object-contain px-2"
          loading="lazy"
        />
      </div>
    </div>
  );
}