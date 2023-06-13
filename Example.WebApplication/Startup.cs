using Byndyusoft.Example.WebApplication.MessageHandlers;
using Byndyusoft.Net.Kafka.Extensions;
using KafkaFlow;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IExampleService, ExampleService>()
                .AddKafkaBus(Configuration, name => name.Name!.Contains("Example.WebApplication"))
                .AddControllers();
            
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime lifetime)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            var kafkaBus = app.ApplicationServices.CreateKafkaBus();
            lifetime.ApplicationStarted.Register(() => kafkaBus.StartAsync(lifetime.ApplicationStopped));
        }
    }
}