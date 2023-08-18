namespace Byndyusoft.Net.Kafka.Api.Contracts
{
    using System;

    public class EntityCreation
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = default!;
    }
}