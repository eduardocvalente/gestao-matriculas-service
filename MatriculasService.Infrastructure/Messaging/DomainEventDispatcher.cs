using MatriculasService.Application.Interfaces;
using MatriculasService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MatriculasService.Infrastructure.Messaging;

internal sealed class DomainEventNotification<TEvent> : INotification
    where TEvent : IDomainEvent
{
    public TEvent DomainEvent { get; }

    public DomainEventNotification(TEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IPublisher publisher, ILogger<DomainEventDispatcher> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task DispatchAsync(IReadOnlyList<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var @event in events)
        {
            _logger.LogInformation(
                "Publicando domain event. {EventType} {EventId} {OccurredAt}",
                @event.GetType().Name, @event.EventId, @event.OccurredAt);

            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(@event.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, @event)!;
            await _publisher.Publish(notification, cancellationToken);
        }
    }
}
