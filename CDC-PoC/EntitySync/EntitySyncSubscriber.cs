using R365.Messaging.Subscriber;

namespace CDC_PoC.EntitySync;

public class EntitySyncSubscriber : IBatchSubscriber<EntitySyncDto>
{
    private readonly ILogger<EntitySyncSubscriber> _logger;

    public EntitySyncSubscriber(ILogger<EntitySyncSubscriber> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IReadOnlyCollection<EntitySyncDto> messages, CancellationToken cancellationToken)
    {
        foreach (var message in messages)
        {
            _logger.LogInformation("Received message: {message}", message);
        }

        return Task.CompletedTask;
    }
}