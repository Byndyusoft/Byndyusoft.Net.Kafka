namespace Byndyusoft.Net.Kafka.Logging
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using KafkaFlow;
    using MaskedSerialization.Newtonsoft.Helpers;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    internal class ProducedMessageLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ProducedMessageLoggingMiddleware> _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public ProducedMessageLoggingMiddleware(ILogger<ProducedMessageLoggingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializerSettings = MaskedSerializationHelper.GetSettingsForMaskedSerialization()
                .ApplyDefaultSettings();
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            _logger.LogInformation(
                "{TraceEventName} Parameters: MessageBody = {MessageBody}",
                "Producing message",
                JsonConvert.SerializeObject(context.Message.Value, _serializerSettings)
            );
            return next(context);
        }
    }
}