// Dev-only helper for testing MaxMind country/currency resolution locally,
// where every request resolves to a private IP the geo lookup can't map to a
// real country. Mirrors the backend's own dev override
// (CountryResolverService: ?country=<Country enum name> or X-Dev-Country) -
// set it once via the URL (e.g. ?country=Egypt) and it persists across SPA
// navigation without needing the param on every link. import.meta.env.DEV is
// false in a production build, so this can never activate outside `npm run
// dev`.
const STORAGE_KEY = "devCountry";

export function getDevCountryOverride() {
  if (!import.meta.env.DEV) {
    return null;
  }

  const fromUrl = new URLSearchParams(window.location.search).get("country");

  if (fromUrl !== null) {
    if (fromUrl) {
      sessionStorage.setItem(STORAGE_KEY, fromUrl);
    } else {
      sessionStorage.removeItem(STORAGE_KEY);
    }
    return fromUrl || null;
  }

  return sessionStorage.getItem(STORAGE_KEY);
}
