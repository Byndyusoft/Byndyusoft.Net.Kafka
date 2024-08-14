namespace Byndyusoft.Net.Kafka.Middlewares
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using KafkaFlow;

    public class ConsumedMessageLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ConsumedMessageLoggingMiddleware> _logger;

        public ConsumedMessageLoggingMiddleware(ILogger<ConsumedMessageLoggingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            _logger.LogInformation(
                "{TraceEventName} Parameters: MessageBody = {MessageBody}",
                "Consuming message",
                Encoding.UTF8.GetString((byte[])context.Message.Value)
            );
            return next(context);
        }
    }
}