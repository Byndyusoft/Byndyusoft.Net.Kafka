namespace Byndyusoft.Net.Kafka.Middlewares;

using System;
using System.Threading.Tasks;
using Extensions;
using KafkaFlow;
using OpenTracing;
using OpenTracing.Tag;

internal class ConsumeMessageTracingMiddleware : IMessageMiddleware
{
    private readonly ITracer _tracer;

    public ConsumeMessageTracingMiddleware(ITracer tracer)
    {
        _tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));
    }

    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        var spanContext = _tracer.CreateSpanContextFromMessageContext(context);
        var spanBuilder = _tracer
            .BuildSpan("consume")
            .WithTag(Tags.SpanKind.Key, Tags.SpanKindConsumer)
            .AsChildOf(spanContext);

        using (spanBuilder.StartActive())
        {
            _tracer.ActiveSpan.SetMessageContext(context);
            await next(context).ConfigureAwait(false);
        }
    }
}