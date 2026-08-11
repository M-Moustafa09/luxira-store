const STORAGE_KEY = "guestId";

export function getGuestId() {
  let guestId = localStorage.getItem(STORAGE_KEY);

  if (!guestId) {
    guestId = crypto.randomUUID();
    localStorage.setItem(STORAGE_KEY, guestId);
  }

  return guestId;
}

export function clearGuestId() {
  localStorage.removeItem(STORAGE_KEY);
}
