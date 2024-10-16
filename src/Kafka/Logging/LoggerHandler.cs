namespace Byndyusoft.Net.Kafka.Logging
{
    using System;
    using Byndyusoft.MaskedSerialization.Newtonsoft.Helpers;
    using Configuration;
    using KafkaFlow;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    internal class LoggerHandler : ILogHandler
    {
        private readonly ILogger<LoggerHandler> _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public LoggerHandler(ILogger<LoggerHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializerSettings = MaskedSerializationHelper.GetSettingsForMaskedSerialization()
                .ApplyDefaultSettings();
        }

        public void Error(string message, Exception ex, object data)
            => _logger.LogError(
                ex,
                "{TraceEventName} Parameters: Message = {KafkaFlowMessage}, Data = {KafkaFlowData}",
                "Error from KafkaFlow",
                message,
                JsonConvert.SerializeObject(data, _serializerSettings)
            );

        public void Warning(string message, object data)
            => _logger.LogWarning(
                "{TraceEventName} Parameters: Message = {KafkaFlowMessage}, Data = {KafkaFlowData}",
                "Warning from KafkaFlow",
                message,
                JsonConvert.SerializeObject(data, _serializerSettings)
            );

        public void Warning(string message, Exception ex, object data)
            => _logger.LogWarning(
                ex,
                "{TraceEventName} Parameters: Message = {KafkaFlowMessage}, Data = {KafkaFlowData}",
                "Warning from KafkaFlow",
                message,
                JsonConvert.SerializeObject(data, _serializerSettings)
            );

        public void Info(string message, object data)
            => _logger.LogInformation(
                "{TraceEventName} Parameters: Message = {KafkaFlowMessage}, Data = {KafkaFlowData}",
                "Information from KafkaFlow",
                message,
                JsonConvert.SerializeObject(data, _serializerSettings)
            );

        public void Verbose(string message, object data)
            => _logger.LogDebug(
                "{TraceEventName} Parameters: Message = {KafkaFlowMessage}, Data = {KafkaFlowData}",
                "Debug information from KafkaFlow",
                message,
                JsonConvert.SerializeObject(data, _serializerSettings)
            );
    }
}