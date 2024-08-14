namespace Byndyusoft.Net.Kafka.Extensions.DependencyInjection
{
    using System;
    using KafkaFlow;
    using Microsoft.Extensions.DependencyInjection;

    internal class DependencyResolverScope : IDependencyResolverScope
    {
        private readonly IServiceScope _scope;

        public DependencyResolverScope(IServiceScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Resolver = new DependencyResolver(scope.ServiceProvider);
        }

        public IDependencyResolver Resolver { get; }

        public void Dispose() => _scope.Dispose();
    }
}