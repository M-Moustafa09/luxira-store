import { Link } from "react-router-dom";

export default function Button({
  children,
  variant = "primary",
  className = "",
  to,
  ...props
}) {
  const variants = {
    primary: "bg-[#00319D] text-white hover:bg-navy-600",
    outline: "bg-transparent text-[#00319D] border border-[#00319D]",
    soft: "bg-blush-100 text-blush-600",
  };

  const classes = `rounded-md px-2 text-sm font-semibold transition-colors active:scale-[0.98] ${variants[variant]} ${className}`;

  if (to) {
    return (
      <Link to={to} className={classes} {...props}>
        {children}
      </Link>
    );
  }

  return (
    <button className={classes} {...props}>
      {children}
    </button>
  );
}
