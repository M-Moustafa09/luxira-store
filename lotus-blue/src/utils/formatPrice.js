export function formatPrice(value) {
  return new Intl.NumberFormat('ar-SA', { maximumFractionDigits: 0 }).format(value)
}
