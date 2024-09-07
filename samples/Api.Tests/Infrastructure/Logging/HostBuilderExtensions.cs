namespace MusicalityLabs.ComposerAssistant.Storage.Api.Tests.Infrastructure.Logging
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Moq;

    public  static class HostBuilderExtensions
    {
        public static IHostBuilder UseMockLogger(this IHostBuilder builder, Mock<ILogger> mockLogger)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);

            return builder.ConfigureLogging(x => x.Services.Replace(ServiceDescriptor.Singleton(mockLoggerFactory.Object)));
        }
    }
}