namespace Byndyusoft.Net.Kafka.Tests.Common.Infrastructure;

using Microsoft.Extensions.Configuration;
using System.IO;

public class ConfigurationFactory
{
    public static IConfiguration Create()
        => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();
}