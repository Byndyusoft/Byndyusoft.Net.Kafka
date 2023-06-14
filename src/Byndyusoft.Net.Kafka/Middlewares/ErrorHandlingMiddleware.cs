using System;
using System.Threading.Tasks;
using Byndyusoft.Net.Kafka.Extensions;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace Byndyusoft.Net.Kafka.Middlewares
{
    internal class ErrorHandlingMiddleware : IMessageMiddleware
    {
        private readonly ITracer _tracer;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

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