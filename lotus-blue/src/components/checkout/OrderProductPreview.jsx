export default function OrderProductPreview({ image }) {
  return (
    <div className="h-20 w-11 overflow-hidden rounded-md border border-[#ECECEC] ">
      <img
        src={image}
        alt=""
        className="h-full w-full object-cover"
      />
    </div>
  );
}