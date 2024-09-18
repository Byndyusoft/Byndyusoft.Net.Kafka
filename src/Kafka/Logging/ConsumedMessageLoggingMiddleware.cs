namespace Byndyusoft.Net.Kafka.Logging
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using KafkaFlow;
    using MaskedSerialization.Newtonsoft.Helpers;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class ConsumedMessageLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<ConsumedMessageLoggingMiddleware> _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public ConsumedMessageLoggingMiddleware(ILogger<ConsumedMessageLoggingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializerSettings = MaskedSerializationHelper.GetSettingsForMaskedSerialization()
                .ApplyDefaultSettings();
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            _logger.LogInformation(
                "{TraceEventName} Parameters: MessageBody = {MessageBody}",
                "Consuming message",
                JsonConvert.SerializeObject(context.Message.Value, _serializerSettings)
            );
            return next(context);
        }
    }
}