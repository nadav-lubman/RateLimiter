using MizeRateLimiter.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MizeRateLimiter.Tests
{
    [TestClass]
    public class RateLimiterTests 
    {
        [TestMethod]
        public async Task Perform_InvokesActionAndAppliesAllRateLimitRules()
        {
            // Arrange
            var rule1 = new Mock<IRateLimitRule>();
            var rule2 = new Mock<IRateLimitRule>();

            rule1.Setup(r => r.WaitForAvailabilityAsync()).Returns(Task.CompletedTask);
            rule2.Setup(r => r.WaitForAvailabilityAsync()).Returns(Task.CompletedTask);

            var rateLimiter = new RateLimiter<string>(
                async arg => await Task.CompletedTask,
                new[] { rule1.Object, rule2.Object }
            );

            // Act
            await rateLimiter.Perform("some input");

            // Assert
            rule1.Verify(r => r.WaitForAvailabilityAsync(), Times.Once);
            rule2.Verify(r => r.WaitForAvailabilityAsync(), Times.Once);
            rule1.Verify(r => r.RecordCall(It.IsAny<DateTime>()), Times.Once);
            rule2.Verify(r => r.RecordCall(It.IsAny<DateTime>()), Times.Once);
        }


        [TestMethod]
        public async Task Perform_ExecutesAllConcurrentCallsSuccessfully()
        {
            // Arrange
            int numOfTasks = 10;
            int executedCount = 0;
            var limiter = new RateLimiter<int>(
                async _ => { Interlocked.Increment(ref executedCount); await Task.Delay(30); },
                new[] { new RateLimitRule(100, TimeSpan.FromSeconds(1)) }
            );

            // Act
            var tasks = Enumerable
                .Range(0, numOfTasks)
                .Select(limiter.Perform);
            await Task.WhenAll(tasks);

            // Assert
            Assert.AreEqual(numOfTasks, executedCount);
        }
    }
}
