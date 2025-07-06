using MizeRateLimiter.Interfaces;
using System.Collections.Concurrent;

namespace MizeRateLimiter
{
    public class RateLimitRule : IRateLimitRule
    {
        public int MaxCalls { get; }
        public TimeSpan TimeWindow { get; }

        private readonly ConcurrentQueue<DateTime> m_callTimestamps = new();

        public RateLimitRule(int maxCalls, TimeSpan timeWindow)
        {
            MaxCalls = maxCalls;
            TimeWindow = timeWindow;
        }

        public async Task WaitForAvailabilityAsync()
        {
            bool isAvailable = false;

            while (!isAvailable)
            {
                while (m_callTimestamps.TryPeek(out var timestamp) &&
                       DateTime.UtcNow - timestamp > TimeWindow)
                {
                    m_callTimestamps.TryDequeue(out _);
                }

                if (m_callTimestamps.Count < MaxCalls)
                {
                    isAvailable = true;
                }
                else if (m_callTimestamps.TryPeek(out DateTime oldest))
                {
                    var delay = TimeWindow - (DateTime.UtcNow - oldest);
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay);
                    }
                }
            }
        }

        public void RecordCall(DateTime time)
        {
            m_callTimestamps.Enqueue(time);
        }
    }
}
