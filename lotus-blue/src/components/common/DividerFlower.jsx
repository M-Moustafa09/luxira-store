export default function DividerFlower({ className = "" }) {
  return (
    <svg
      viewBox="0 0 200 40"
      className={className}
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* left line */}
      <line
        x1="0"
        y1="20"
        x2="78"
        y2="20"
        stroke="currentColor"
        strokeWidth="1"
      />

      {/* right line */}
      <line
        x1="122"
        y1="20"
        x2="200"
        y2="20"
        stroke="currentColor"
        strokeWidth="1"
      />

      {/* flower */}
      <g stroke="currentColor" strokeWidth="1" fill="none">
        <path d="M100 20 C94 14, 94 8, 100 4 C106 8, 106 14, 100 20 Z" />
        <path d="M100 20 C106 14, 112 14, 116 20 C112 26, 106 26, 100 20 Z" />
        <path d="M100 20 C94 26, 94 32, 100 36 C106 32, 106 26, 100 20 Z" />
        <path d="M100 20 C94 14, 88 14, 84 20 C88 26, 94 26, 100 20 Z" />
        <path d="M100 20 m-4 -4 C98 12, 102 12, 104 16" />
      </g>

      <circle
        cx="100"
        cy="20"
        r="2.5"
        stroke="currentColor"
        strokeWidth="1"
        fill="none"
      />
    </svg>
  );
}