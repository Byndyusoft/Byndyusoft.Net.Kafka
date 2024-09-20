namespace MusicalityLabs.ComposerAssistant.Storage.Api.Tests.Infrastructure.Logging
{
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Linq;
    using Xunit;

    public static class MockLoggerExtensions
    {
        public static Mock<ILogger> CreateMockLogger()
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            return mockLogger;
        }

        public static Mock<TLogger> VerifyNoErrorsWasLogged<TLogger>(this Mock<TLogger> mockLogger) where TLogger : class, ILogger
        {
            Assert.Null(
                mockLogger.Invocations.SingleOrDefault(
                    x => Equals(x.Arguments[0], LogLevel.Error)
                )
            );
            return mockLogger;
        }

        public static Mock<TLogger> VerifyInformationMessageWasLogged<TLogger>(this Mock<TLogger> mockLogger, string message) where TLogger : class, ILogger
        {
            Assert.Null(
                mockLogger.Invocations.SingleOrDefault(
                    x => Equals(x.Arguments[0], LogLevel.Information)
                         && x.Arguments.Count >= 3 && Equals(x.Arguments[2], message)
                )
            );
            return mockLogger;
        }
    }
}