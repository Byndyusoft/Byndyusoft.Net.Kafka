# Byndyusoft.Net.Kafka

Библиотека для работы с Kafka.

## Installing

```shell
dotnet add package Byndyusoft.Net.Kafka
```

## Usage

### Producer:
```c#
public class ExampleProducer : KafkaProducerBase<ExampleMessageDto>
    {
        public ExampleProducer(IProducerAccessor producers) : base(producers, "Title")
        {
        }

        public override string Topic  => "topic";
        public override string ClientName => "example_client";

        public override string KeyGenerator(ExampleMessageDto message)
        {
            return message.Guid.ToString();
        }
    }
```

### Consumer:
```c#
public class ExampleConsumer : IKafkaConsumer
    {
        public ExampleConsumer(ExampleMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }   
        public string Topic => "topic";

        public string GroupName => "example_group";
        public IMessageHandler MessageHandler { get; }
    }
    
public sealed class ExampleMessageHandler : IMessageHandler<ExampleMessageDto>
{
    public async Task Handle(IMessageContext context, ExampleMessageDto message)
    {
        // Implementation
    }
}
```

### DI:
```c#
services.AddKafkaBus(Configuration, name => name.Name!.Contains("Example.WebApplication"));
```

### Configuration
```json
{
  "KafkaSettings" : {
    "Hosts": [
      "localhost:9092"
    ],
    "SecurityInformationEnabled" : false,
    "Prefix" : "example"
  },
  "KafkaSecurityInformationSettings" : {
    "Username" : "username",
    "Password" : "password",
    "SaslMechanism" : "ScramSha512",
    "SecurityProtocol" : "SaslPlaintext"
  }
}
```
