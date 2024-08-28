namespace MusicalityLabs.Storage.Api
{
    using System;
    using Byndyusoft.Net.Kafka;
    using Byndyusoft.Net.Kafka.Configuration;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
           services
                .AddControllers()
            .AddTracing();
            services.AddSwaggerGen();

            services.AddKafkaBus(_configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>());
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

            app.UseKafkaBus(lifetime);
        }
    }
}