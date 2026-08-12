import { getGuestId } from "./guestId.js";
import { getAccessToken } from "./authToken.js";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

async function request(path, { method = "GET", body } = {}) {
  const accessToken = getAccessToken();

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers: {
      "X-Guest-Id": getGuestId(),
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
      ...(body ? { "Content-Type": "application/json" } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    throw new Error(await extractErrorMessage(response, path));
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

// Surfaces the backend's ProblemDetails message (e.g. "wrong password",
// "email already registered") instead of a generic status-code string,
// since forms like login/register need the real reason to show the user.
async function extractErrorMessage(response, path) {
  try {
    const problem = await response.json();
    const firstValidationError = problem?.errors && Object.values(problem.errors)[0]?.[0];
    return firstValidationError ?? problem?.detail ?? problem?.title ?? `API request failed: ${response.status} ${path}`;
  } catch {
    return `API request failed: ${response.status} ${path}`;
  }
}

export function apiGet(path) {
  return request(path);
}

export function apiPost(path, body) {
  return request(path, { method: "POST", body });
}

export function apiPut(path, body) {
  return request(path, { method: "PUT", body });
}

export function apiDelete(path) {
  return request(path, { method: "DELETE" });
}
