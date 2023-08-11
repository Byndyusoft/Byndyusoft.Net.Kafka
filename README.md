# Byndyusoft.Net.Kafka

Высокоуровневая библиотека для работы с kafka. 
Базируется на KafkaFlow, которая в свою очередь основана на Confluent Kafka Client.

## Quickstart

1. Создаем topics с которыми будет работать приложение, используя конвенцию именования:
"{проект}.{сущность}.{событие с ней произошедшее}"

Примеры:
project.notification.status_change
project.notification.awaiting_official_sending
project.notification.official_sent

project.portfolio.input_data
project.portfolio.precalculation_result

Если прямо сейчас у вас нет времени погружаться в настройки topics:

## План
1. CooperativeSticky - зафиксировать, что эта настройка означает
2. Описать PartitionAssignmentStrategy
3. 

## Настройки brocker:
- min.insync.replicas = 2 - минимальное количество согласованных реплик прежде чем producer.acks вернет ok()

## Настройки toptic:
- replication factor = 3
  https://www.conduktor.io/kafka/kafka-topics-choosing-the-replication-factor-and-partitions-count#How-to-choose-the-number-of-partitions-in-a-Kafka-topic-1 
- log retention 
  https://www.conduktor.io/kafka/kafka-topic-configuration-log-retention  
	- retention.ms= 1814400000 - логи хранятся три недели. Обсуждаемый параметр, если Инфинитум считает, что скоро реакции должна быть выше - можно уменьшить. Указываем в ms, потому что Kafka CLI работает с ms
	- retention.bytes = -1 - нет ограничения по размеру лога
- unclean.leader.election.enable = false - запрещаем рассогласованным репликам становиться ведущими. Это снижает доступность, т.к мы будем ждать восстановления ведущей реплики, но гарантируем, что данные останутся согласованными
  https://www.datadoghq.com/blog/kafka-at-datadog/#unclean-leader-elections-to-enable-or-not-to-enable
- Нужно создать столько partitions, сколько может потребоваться и больше не менять их число, иначе kafka не гарантирует последовательную обработку сообщений (стр. 85)
  +кратно количеству брокеров

## Настройки producer:
- max.in.flight.requests.per.connection = 1 - количество сообщений, которое может быть отправлено брокеру, не дожидаясь ответа

## Настройки Producer в коде
- Botstrap servers - указываем адреса трех брокеров, чтобы в случае падения одного из них, мы пробовали подключиться к следующему
- acks = all - ответ придет после синхронизации с количеством реплик, указанных в min.insync.replicas
- retries = 3 - количество попыток повторной отправки сообщений
  
- ProducerConfig 
	- enable.idempotence=true  -- it enables an idempotent producer which ensures that exactly one copy of each message is written to the brokers
	- Leader not Available - нужно обрабатывать эту ошибку и делать ретрай
	- retry.backoff.ms = 1000 - время между retries в ms. 1000 - рекомендация от alibabacloud
	- transactional.id
	- количество партиций - brockers count * 3 = 9
	 https://www.conduktor.io/kafka/kafka-topics-choosing-the-replication-factor-and-partitions-count#How-to-choose-the-number-of-partitions-in-a-Kafka-topic-0
- KafkaConsumer
	- подписываются с помощью Subscribe(), а не Assign() + константы для групп
	- commitSync - это уменьшит пропускную способность, но это гарантирует, что каждое сообщение будет последовательно обработано (стр. 105)
	- СonsumerRebalanceListener.OnPartitionsRevoked - что будет делать Consumer, перед ребалансировкой, но после получения сообщения. В нем нужно зафиксировать смещение, чтобы следующий Consumer начал обработку сообщений с корректного места
	- auto.offset.reset - last - consumer при старте читает с последнего офсета
	- в случае retry - ставим на паузу обработку и пробуем еще раз. Или кладем dead-letters-queue, но при этом нам надо разделять топики по операциям (p. 175)
- Дубли сообщений (Idempotency || exactly once):
1. enable.auto.commit = false. Коммитим вручную после обработки сообщения, а не сразу при получении 
2. Отслеживаем все полученные сообщения и удаляем дубликаты `PROCESSED_MESSAGE`
3. ... или делаем batch = 1
4. max.poll.interval.ms - максимальное время обработки сообщения * 2??
5. клиент может у себя в хранилище сохранять последний обработанный offset и по нему фильтровать дубли
- Обработка ошибок 
    KafkaProducer различает ошибки, которые можно ретраить и которые нельзя

Работа через командную строку:
https://www.conduktor.io/kafka/how-to-start-kafka-using-docker

Тех долг
1. Validating Configuration (p. 176)
2. Validating Applications (p. 177)
3. Kafka Consumer:
   Техдолг:
	  - нужно будет переписать на асинхронную + синхронную фиксацию (стр.106)
	  - самый надежный вариант - хранить смещения в бд (стр. 111)
4. Тех. долг - обрабатывать NotEnoughReplicasException
5.  Принять решение, что делать с Nonretriable broker errors
6. почитать про транзакции

https://www.kadeck.com/blog/how-many-partitions-do-i-need-in-apache-kafka
https://www.confluent.io/blog/how-choose-number-topics-partitions-kafka-cluster/
https://www.baeldung.com/learn-spring-course

https://thecloudblog.net/post/building-reliable-kafka-producers-and-consumers-in-net/

Соглашения:
1. devops создают топики, т.к они должны определять количество partitions

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
    "KafkaSecurityInformationSettings" : {
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
    IHostApplicationLifetime lifetime)
{
	app.UseKafkaBus(app);
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
public class SomethingHappenedEventsProducer : KafkaProducerBase<SomeEvent>
{
    public SomethingHappenedEventsProducer(IProducerAccessor producers) 
		: base(producers, nameof(SomethingHappenedEventsProducer))
    {
    }

    public override string Topic  => "project.entity.some_event";
    
    public override string KeyGenerator(ExampleMessageDto message)
		=> SomeEvent.Id.ToString();
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
public class SomethingHappenedEventsConsumer : IKafkaConsumer
{
	public SomethingHappenedEventsConsumer(SomethingHappenedMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }   
	
        public string Topic => "project.entity.some_event";
        
        public IMessageHandler MessageHandler { get; }
}

public sealed class SomethingHappenedMessageHandler : IMessageHandler<SomeEvent>
{
        public Task Handle(IMessageContext context, SomeEvent someEvent)
        	=> Task.FromResult(someEvent)
}
```
