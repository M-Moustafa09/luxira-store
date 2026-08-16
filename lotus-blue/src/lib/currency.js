// Maps a currency code (as returned by the API - Product/Cart/Order
// responses) to the short Arabic label used throughout the storefront next
// to a price, mirroring the backend's CountryCurrency mapping
// (Luxira.Domain/Common/CountryCurrency.cs) plus the USD fallback used for
// visitors outside the 16 supported countries.
const CURRENCY_LABELS = {
  USD: "$",
  SAR: "ر.س",
  AED: "د.إ",
  JOD: "د.أ",
  BHD: "د.ب",
  DZD: "د.ج",
  IQD: "د.ع",
  KWD: "د.ك",
  MAD: "د.م",
  TRY: "₺",
  TND: "د.ت",
  OMR: "ر.ع",
  ILS: "₪",
  QAR: "ر.ق",
  LBP: "ل.ل",
  LYD: "د.ل",
  EGP: "ج.م",
};

export function getCurrencyLabel(currencyCode) {
  return CURRENCY_LABELS[currencyCode] ?? currencyCode ?? "ر.س";
}
