namespace Byndyusoft.Net.Kafka.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Extensions;
    using KafkaFlow;
    using Microsoft.Extensions.Logging;
    using OpenTracing;

    internal class ErrorHandlingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly ITracer _tracer;

        public ErrorHandlingMiddleware(ITracer tracer, ILoggerFactory loggerFactory)
        {
            _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _tracer.ActiveSpan.SetException(ex);
                _logger.LogError(ex, "Unexpected error");
            }
        }
    }
}