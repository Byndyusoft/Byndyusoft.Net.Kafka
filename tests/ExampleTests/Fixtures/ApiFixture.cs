namespace Byndyusoft.Net.ExampleTests.Fixtures
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestPlatform.TestHost;
    using Moq;
    using OpenTracing;
    using OpenTracing.Mock;

    public class ApiFixture : WebApplicationFactory<Program>
    {
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

        public T GetService<T>()
        {
            return Services.GetService<T>()!;
        }
    }
}