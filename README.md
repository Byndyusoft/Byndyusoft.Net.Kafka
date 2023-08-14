# Byndyusoft.Net.Kafka

Высокоуровневая библиотека для работы с kafka. 
Базируется на KafkaFlow, которая в свою очередь основана на Confluent Kafka Client.

## Quickstart

1. Создаем topic с которыми будет работать приложение, используя конвенцию именования:
"{проект}.{сущность}.{событие с ней произошедшее}"

Примеры:
project.notification.status_change
project.notification.awaiting_official_sending
project.notification.official_sent

project.portfolio.input_data
project.portfolio.precalculation_result

Если прямо сейчас у вас нет времени погружаться в настройки topics:

| Настройка           | Описание                                                                                                | Значение                           |
| ------------------- | ------------------------------------------------------------------------------------------------------- | ---------------------------------- |
| partitions          | Количество партиций                                                                                     | 9                                  |
| replication-factor  | Количество реплик каждой из партиций                                                                    | 3                                  |
| min.insync.replicas | Число реплик, которые должны быть синхронизированы, чтобы можно было продолжить запись                  | 2                                  |
| retention.ms        | Определяет максимальный возраст сообщения, после превышения которого следует его удалить                | Время разбора инцидента поддержкой |
| retry.backoff.ms    | Задержка (в миллисекундах) между повторными попытками отправки сообщений в случае возникновения ошибки. | 1000                               | 


2. Устанавливаем пакет 
```shell
dotnet add package Byndyusoft.Net.Kafka
```

2. Инициализируем настройки kafka в appsettings.json
```json
{
  "KafkaSettings" : {
    "Hosts": [
      "some-host:9092"
    ],
    "Prefix" : "some-prefix",
    "SecurityInformation" : {
	    "Username" : "username",
	    "Password" : "password",
	    "SaslMechanism" : "ScramSha512",
	    "SecurityProtocol" : "SaslPlaintext"
	}
}
```

3. Используем расширение AddKafkaBus, которое регистрирует все необходимые зависимости для работы библиотеки, в т.ч:
- настройки Kafka из appsettings.json;
- отправители сообщений, которые являются потомками KafkaProducerBase;
- потребители сообщений, которые реализуют IKafkaConsumer;
- обработчики входящих сообщений, которые реализуют IMessageHandler
```c#
public void ConfigureServices(IServiceCollection services)
{
	services.AddKafkaBus(Configuration);
}
```

4. Говорим запустить kafka до момента остановки работы приложения
```c#
public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env,
    IHostApplicationLifetime lifetime
)
{
	app.UseKafkaBus(lifetime);
}
```

## Producer

1. Ожидает подтверждения от всех синхронизированных реплик, прежде, чем считать сообщение успешно отправленным. 
2. Идемпотентность: cемантика exactly-once с сохранением порядка доставки для каждого раздела.
3. Если сообщение не удалось отправить, producer будет повторять попытку до 3 раз с задержкой в 1 секунду между каждой попыткой

| Настройка           | Описание           | Значение            		| 
| ------------------- | ------------------ | -----------------------------------| 
| enable.idempotence  | Гарантирует запись только одного сообщения в конкретную партицию одного топика | true |     
| max.in.flight.requests.per.connection | Общее число неподтверждённых брокером запросов для одного клиента | 1 |        
| acks                | После скольких acknowledge лидеру кластера необходимо считать сообщение успешно записанным | all (значение из min.insync.replicas) | 

Конвенция наименования: 
"{KafkaSettings.Prefix}.{Producer}.{Producer.Topic}".ToSnakeCase()

Примеры:
project.something_happened_events_producer.project.entity.some_event

```c#
public class EntityCreationEventsProducer : KafkaProducerBase<EntityCreation>
{
    public EntityCreationEventsProducer(IProducerAccessor producers) 
        : base(producers, nameof(EntityCreationEventsProducer))
    {
    }

    public override string Topic => "project.entity.creation";
    
    public override string KeyGenerator(EntityCreation entityCreation)
        => entityCreation.Id.ToString();
}
```

## Consumer

1. Начинает чтение сообщений с самого раннего доступного смещения в топике, т.е с самого старого сообщения, которое еще не было удалено из топика. 
2. Если сообщение не удалось обработать, consumer будет повторять попытку до 3 раз с задержкой равной retryCount^2

Конвенция наименования: 
"{KafkaSettings.Prefix}.{Consumer}.{Consumer.Topic}".ToSnakeCase()

Примеры:
project.something_happened_events_producer.project.entity.some_event

```c#
public class EntityCreationEventsConsumer : IKafkaConsumer
{
    public EntityCreationEventsConsumer(EntityCreationMessageHandler messageHandler)
    {
        MessageHandler = messageHandler;
    }

    public string Topic => "project.entity.creation";

    public IMessageHandler MessageHandler { get; }
}

public class EntityCreationMessageHandler : IMessageHandler<EntityCreation>
{
    public Task Handle(IMessageContext context, EntityCreation someEvent)
    {
        Console.WriteLine(
            "Partition: {0} | Offset: {1} | Message: {2}",
            context.ConsumerContext.Partition,
            context.ConsumerContext.Offset,
            someEvent.Text
        );

        return Task.FromResult(someEvent);
    }
}
```
