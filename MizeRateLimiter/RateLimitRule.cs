using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MizeRateLimiter
{
    public class RateLimitRule
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
            while (true)
            {
                // Remove timestamps outside the window
                while (m_callTimestamps.TryPeek(out var timestamp) &&
                    DateTime.UtcNow - timestamp > TimeWindow)
                {
                    m_callTimestamps.TryDequeue(out _);
                }

                if (m_callTimestamps.Count < MaxCalls)
                {
                    return;
                }


                m_callTimestamps.TryPeek(out DateTime oldest);
                var delay = TimeWindow - (DateTime.UtcNow - oldest);
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay);
                }
                    
            }
        }

        public void RecordCall(DateTime time)
        {
            m_callTimestamps.Enqueue(time);
        }
    }
}
