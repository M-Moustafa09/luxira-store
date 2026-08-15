import { useEffect, useRef } from "react";

// Re-runs `callback` on a fixed interval while `enabled` is true, skipping
// ticks while the tab is backgrounded (document.visibilityState !== "visible")
// so an idle tab doesn't keep hitting the API. Used for the small set of
// pages where an admin action elsewhere (a new campaign, an order status
// change) should reach the customer without a manual refresh.
export function usePolling(callback, intervalMs, enabled = true) {
  const callbackRef = useRef(callback);
  callbackRef.current = callback;

  useEffect(() => {
    if (!enabled) {
      return;
    }

    const id = setInterval(() => {
      if (document.visibilityState === "visible") {
        callbackRef.current();
      }
    }, intervalMs);

    return () => clearInterval(id);
  }, [intervalMs, enabled]);
}
