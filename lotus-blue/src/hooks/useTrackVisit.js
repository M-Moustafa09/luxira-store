import { useEffect } from "react";
import { apiPost } from "../lib/apiClient.js";

const SESSION_FLAG_KEY = "visitTracked";

// Records exactly one storefront visit per browser tab session - not per page
// load/route change, which would really be a page-view count rather than
// "a visit" the way the admin dashboard's number is meant to read.
// sessionStorage naturally resets per tab and clears on close, which is
// exactly the boundary a "visit" should reset on.
export function useTrackVisit() {
  useEffect(() => {
    if (sessionStorage.getItem(SESSION_FLAG_KEY)) {
      return;
    }

    sessionStorage.setItem(SESSION_FLAG_KEY, "1");

    // Best-effort - a failed visit-tracking call should never disrupt browsing.
    apiPost("/api/analytics/visit").catch(() => {});
  }, []);
}
