namespace MizeRateLimiter.Interfaces
{
    public interface IRateLimiter<TArg>
    {
        Task Perform(TArg argument);
    }
}
