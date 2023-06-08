using Byndyusoft.Example.WebApplication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OpenTracing;
using OpenTracing.Mock;
using Xunit.Abstractions;

namespace Byndyusoft.ExampleTests.Fixtures;

public class ApiFixture : WebApplicationFactory<Program>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ApiFixture(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public ITracer Tracer { get; } = new MockTracer();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(ConfigureTestServices);
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddSingleton(Tracer);
    }
}