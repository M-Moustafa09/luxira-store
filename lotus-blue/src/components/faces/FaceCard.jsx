export default function FaceCard({
  image,
  title,
  icon: Icon,
  iconColor = "#00319D",
}) {
  return (
    <div className="overflow-hidden rounded-[12px] bg-[#FFF2F3]">
      <div className="h-[120px] w-full overflow-hidden">
        <img
          src={image}
          alt={title}
          className="h-full w-full object-cover"
        />
      </div>

      <div className="flex h-[40px] items-center justify-center gap-2 bg-[#fae5e6]">
        <span className="text-[16px] font-medium text-[#00319D]">
          {title}
        </span>
        <Icon
          size={20}
          strokeWidth={1}
          style={{ color: iconColor }}
        />
      </div>
    </div>
  );
}