namespace Byndyusoft.Net.Kafka.Extensions
{
    using System;
    using Newtonsoft.Json;

    internal static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings ApplyDefaultSettings(this JsonSerializerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            settings.TypeNameHandling = TypeNameHandling.Auto;
            return settings;
        }

        public static JsonSerializerSettings DefaultSettings { get; } = new JsonSerializerSettings().ApplyDefaultSettings();
    }
}