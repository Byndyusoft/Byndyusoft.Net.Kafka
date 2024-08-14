namespace Byndyusoft.Net.Kafka.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using KafkaFlow;
    using Microsoft.Extensions.DependencyInjection;

    internal class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object Resolve(Type type)
            => _serviceProvider.GetService(type);

        public IEnumerable<object?> ResolveAll(Type type)
            => _serviceProvider.GetServices(type);

        public IDependencyResolverScope CreateScope()
            => new DependencyResolverScope(_serviceProvider.CreateScope());
    }
}