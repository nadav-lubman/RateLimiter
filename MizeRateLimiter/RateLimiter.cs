using MizeRateLimiter.Interfaces;

namespace MizeRateLimiter
{
    public class RateLimiter<TArg> : IRateLimiter<TArg>
    {
        private readonly Func<TArg, Task> m_action;
        private readonly List<IRateLimitRule> m_rules;
        private readonly SemaphoreSlim m_semaphore = new(1, 1);

        public RateLimiter(Func<TArg, Task> action, IEnumerable<IRateLimitRule> rules)
        {
            m_action = action ?? throw new ArgumentNullException(nameof(action));
            m_rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
            if (m_rules.Count == 0)
            {
                throw new ArgumentException("At least one rate limit rule must be provided.");
            }
        }

        public async Task Perform(TArg argument)
        {
            await m_semaphore.WaitAsync();

            try
            {
                foreach (IRateLimitRule rule in m_rules)
                {
                    await rule.WaitForAvailabilityAsync();
                }
            }
            finally
            {
                m_semaphore.Release();
            }

            await m_action(argument);

            DateTime time = DateTime.UtcNow;
            m_rules.ForEach(rule => rule.RecordCall(time));
        }
    }
}
