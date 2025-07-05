namespace MizeRateLimiter.Interfaces
{
    public interface IRateLimitRule
    {
        Task WaitForAvailabilityAsync();
        void RecordCall(DateTime time);
    }
}
