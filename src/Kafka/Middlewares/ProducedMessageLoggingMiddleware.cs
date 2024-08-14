﻿namespace Byndyusoft.Net.Kafka.Middlewares
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using KafkaFlow;

    internal class ProducedMessageLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ProducedMessageLoggingMiddleware> _logger;

        public ProducedMessageLoggingMiddleware(ILogger<ProducedMessageLoggingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            _logger.LogInformation(
                "{TraceEventName} Parameters: MessageBody = {MessageBody}",
                "Producing message",
                Encoding.UTF8.GetString((byte[])context.Message.Value)
            );
            return next(context);
        }
    }
}