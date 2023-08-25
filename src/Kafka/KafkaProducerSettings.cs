using System;

namespace Byndyusoft.Net.Kafka;

/// <summary>
///     Settings to connection with Kafka
/// </summary>
public class KafkaProducerSettings
{
    private const int TwentyMegabytes = 20 * 1024 * 1024;
    private static readonly int OneSecondMs = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;

    public KafkaProducerSettings()
    {
        MessageMaxBytes = TwentyMegabytes;
        RetryBackoffMs = OneSecondMs;
        MessageSendMaxRetries = 3;
    }

    public int MessageMaxBytes { get; set; }

    public int RetryBackoffMs { get; set; }

    public int MessageSendMaxRetries { get; set; }
}