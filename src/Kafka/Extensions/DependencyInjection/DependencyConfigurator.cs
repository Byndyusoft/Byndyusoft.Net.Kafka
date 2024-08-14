namespace Byndyusoft.Net.Kafka.Extensions.DependencyInjection
{
    using KafkaFlow;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class DependencyConfigurator : IDependencyConfigurator
    {
        private readonly IServiceCollection _services;

        public DependencyConfigurator(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        private static ServiceLifetime ParseLifetime(InstanceLifetime lifetime)
            => lifetime switch
            {
                InstanceLifetime.Singleton => ServiceLifetime.Singleton,
                InstanceLifetime.Scoped => ServiceLifetime.Scoped,
                InstanceLifetime.Transient => ServiceLifetime.Transient,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

        public IDependencyConfigurator Add(
            Type serviceType,
            Type implementationType,
            InstanceLifetime lifetime
        )
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    implementationType,
                    ParseLifetime(lifetime)
                )
            );
            return this;
        }

        public IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TImplementation),
                    ParseLifetime(lifetime)
                )
            );
            return this;
        }

        public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
            where TService : class
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    typeof(TService),
                    typeof(TService),
                    ParseLifetime(lifetime)
                )
            );
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(TImplementation service)
            where TImplementation : class
        {
            _services.AddSingleton(service);
            return this;
        }

        public IDependencyConfigurator Add<TImplementation>(
            Type serviceType,
            Func<IDependencyResolver, TImplementation> factory,
            InstanceLifetime lifetime
        )
        {
            _services.Add(
                ServiceDescriptor.Describe(
                    serviceType,
                    provider => factory(new DependencyResolver(provider))!,
                    ParseLifetime(lifetime)
                )
            );
            return this;
        }
    }
}