namespace Byndyusoft.Net.Kafka.Middlewares
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using KafkaFlow;
    
    internal class ErrorsLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ErrorsLoggingMiddleware> _logger;

        public ErrorsLoggingMiddleware(ILogger<ErrorsLoggingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Activity.Current!.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "Unexpected error");
            }
        }
    }
}