# RateLimiter README

## Two Ways to Limit Rate

### Sliding Window  
Tracks calls over a rolling time frame (e.g., last 24 hours). It constantly checks recent history before running a call.

- **Pros:** Smooth, consistent limits. No big spikes at period boundaries.  
- **Cons:** Needs to track each call’s timestamp, so a bit more complex.

### Absolute Window  
Limits calls within fixed periods (e.g., from midnight to midnight).

- **Pros:** Simple to implement.  
- **Cons:** Can cause bursts right after the window resets (like at midnight).

## Why Sliding Window?

I chose sliding window because it provides more even control and avoids sudden bursts that happen with absolute windows. It’s better when you want to spread calls evenly over time, especially with strict API limits. The extra complexity is worth it for smoother behavior.
