using Byndyusoft.Example.Domain.Services;
using Byndyusoft.Example.Domain.Services.Interfaces;
using Byndyusoft.Net.Kafka.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Byndyusoft.Example.WebApplication
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IExampleService, ExampleService>()
                .AddKafkaBus(Configuration, name => name.Name!.Contains("Example.Domain"))
                .AddControllers();
            
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime lifetime)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            lifetime.RegisterKafkaBus(app);
        }
    }
}