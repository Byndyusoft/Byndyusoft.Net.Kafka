# Byndyusoft.Net.Kafka

Высокоуровневая библиотека для работы с kafka. 
Базируется на [KafkaFlow](https://github.com/Farfetch/kafkaflow), которая в свою очередь основана на Confluent Kafka Client.

## Quickstart

1. Создаем топик с которыми будет работать приложение, используя конвенцию именования:
"{проект}.{сущность}.{событие с ней произошедшее}"

**Пример**: composer_assistant.entity.creation

Если прямо сейчас у вас нет времени погружаться в настройки топиков:

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
    "Username" : "username",
    "Password" : "password"
}
```

3. Используем расширение AddKafkaBus, которое регистрирует все необходимые зависимости для работы библиотеки, в т.ч:
- отправители сообщений, которые являются потомками `KafkaMessageProducerBase` и помечены атрибутом `KafkaMessageProducerAttribute`;
- обработчики входящих сообщений, которые являются потомками `KafkaMessageHandlerBase` и помечены атрибутом `KafkaMessageProducerAttribute`
```c#
public void ConfigureServices(IServiceCollection services)
{
	services.AddKafkaBus(_configuration.GetSection(nameof(KafkaSettings)).Get<KafkaSettings>());
}
```
Поиск отправителей и обработчиков выполняется автоматически из всех используемых сборок, имя которых имеет общий префикс с вызывающей, префиксом считается последовательность символов имени сборки до первой точки.
**Пример**: вызывающая сборка называется MusicalityLabs.ComposerAssistant.Storage.Api, поэтому поиск будет выполняться во всех сборках, название которых начинается с MusicalityLabs.

4. Говорим запустить kafka до момента остановки работы приложения
```c#
public void Configure(
    IApplicationBuilder app,
    IWebHostEnvironment env,
    IHostApplicationLifetime lifetime
)
{
	app.StartKafkaProcessing(lifetime);
}
```

5. Добавляем телеметрию kafka
```c#
.AddOpenTelemetryTracing(
    builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddKafkaInstrumentation()
        .AddAspNetCoreInstrumentation()
)
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
"{проект}.{сервис}.{топик}".ToSnakeCase()

**Пример**: composer_assistant.storage_api.entity_creation

```c#
[KafkaMessageProducer(topic: "composer_assistant.entity.creation")]
public class EntityCreationEventMessageProducer : KafkaMessageProducerBase<EntityCreation>
{
    public EntityCreationEventMessageProducer(IKafkaMessageSender messageSender) : base(messageSender)
    {
    }

    protected override string KeyGenerator(EntityCreation entityCreation)
        => entityCreation.Id.ToString();
}
```

## Consumer

1. При первом запуске потребителя, если не существует сохраненного смещения (offset), то потребитель будет начинать чтение сообщений с самого начала топика, т.е потребитель будет читать все сообщения, начиная с самого раннего доступного смещения (offset) в топике. 
2. Если сообщение не удалось обработать, consumer будет повторять попытку до 3 раз с задержкой равной 2^retryNumber

Конвенция наименования: 
"{проект}.{сервис}.{топик}".ToSnakeCase()

**Пример**: composer_assistant.storage_api.entity_creation

```c#
[KafkaMessageHandler(topic: "composer_assistant.entity.creation")]
public class EntityCreationMessageHandler : KafkaMessageHandlerBase<EntityCreation>
{
    private readonly ILogger<EntityCreationMessageHandler> _logger;

    public EntityCreationMessageHandler(ILogger<EntityCreationMessageHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override Task Handle(EntityCreation someEvent)
    {
        _logger.LogInformation("Message: {EntityText}", someEvent.Text);
        return Task.FromResult(someEvent);
    }
}
```
