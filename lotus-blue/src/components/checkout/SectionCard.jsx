export default function SectionCard({ title, children }) {
  return (
    <section className="rounded-md border border-[#ECECEC] bg-white px-2 py-1">
      <h2 className="mb-2 text-[12px] text-[#00319D] font-bold">{title}</h2>
      {children}
    </section>
  );
}
