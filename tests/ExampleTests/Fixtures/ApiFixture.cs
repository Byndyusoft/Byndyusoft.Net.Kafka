using Byndyusoft.Example.WebApplication;
using Byndyusoft.Example.WebApplication.MessageHandlers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OpenTracing;
using OpenTracing.Mock;

namespace Byndyusoft.ExampleTests.Fixtures;

public class ApiFixture : WebApplicationFactory<Program>
{
    public ITracer Tracer { get; } = new MockTracer();
    
    public IExampleService ExampleService { get; } = Mock.Of<IExampleService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(ConfigureTestServices);
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services
            .AddSingleton(Tracer)
            .AddSingleton(ExampleService);
    }

    public T GetService<T>()
    {
        return Services.GetService<T>()!;
    }
}