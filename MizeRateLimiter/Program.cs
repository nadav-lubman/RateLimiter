namespace MizeRateLimiter
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            RateLimiter<string> rateLimiter = new(
                async s =>
                {
                    Console.WriteLine($"Calling with {s} at {DateTime.UtcNow}");
                    await Task.Delay(100);
                },
                new[]
                {
                new RateLimitRule(1, TimeSpan.FromSeconds(5)),
                new RateLimitRule(2, TimeSpan.FromSeconds(13))
                }
            );

            IEnumerable<Task> tasks = Enumerable.Range(0, 10)
                .Select(i => rateLimiter.Perform($"Request {i}"))
                .ToArray();

            await Task.WhenAll(tasks);
        }
    }
}