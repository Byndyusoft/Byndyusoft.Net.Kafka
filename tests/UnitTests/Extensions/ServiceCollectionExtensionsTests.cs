namespace Byndyusoft.Net.Kafka.Tests.Extensions;

using Common.Infrastructure;
using Kafka.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddingKafkaAppliesAllRequiredSettings()
    {
        //Arrange
        var configuration = ConfigurationFactory.Create();
        var services = new ServiceCollection();

        //
        services.AddKafkaBus(configuration);

        //Assert
        var x = 1;
    }
}