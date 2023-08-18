namespace Byndyusoft.Net.Kafka.Tests
{
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    public class TestCaseBase : IXunitSerializable
    {
        private readonly JsonSerializerSettings _serializerSetting = new() {TypeNameHandling = TypeNameHandling.All};

        public string? TestId { get; set; }

        public string Description { get; set; }

        public override string ToString()
            => string.IsNullOrWhiteSpace(TestId)
                ? $"{TestId} - {Description}"
                : Description;

        private static void DeepCopy(object from, object to)
        {
            foreach (var propertyInfo in from.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(from);
                propertyInfo.SetValue(to, value);
            }
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            var serialized = info.GetValue<string>(nameof(TestCaseBase));
            var testCaseItem = JsonConvert.DeserializeObject(serialized, _serializerSetting);

            DeepCopy(testCaseItem, this);
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(
                nameof(TestCaseBase),
                JsonConvert.SerializeObject(this, _serializerSetting)
            );
        }
    }
}