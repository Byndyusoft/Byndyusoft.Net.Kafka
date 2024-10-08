﻿namespace Byndyusoft.Net.Kafka.Logging
{
    using System;
    using KafkaFlow;
    using Microsoft.Extensions.Logging;

    internal class LoggerHandler : ILogHandler
    {
        private readonly ILogger<LoggerHandler> _logger;

        public LoggerHandler(ILogger<LoggerHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Error(string message, Exception ex, object data)
            => _logger.LogError(ex, message, data);

        public void Warning(string message, object data)
            => _logger.LogWarning(message, data);

        public void Warning(string message, Exception ex, object data)
            => _logger.LogWarning(ex, message, data);

        public void Info(string message, object data)
            => _logger.LogInformation(message, data);

        public void Verbose(string message, object data)
            => _logger.LogDebug(message, data);
    }
}