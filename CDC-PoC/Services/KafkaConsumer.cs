using System.Text;
using System.Text.Json;
using CDC_PoC.Models;
using Kafka.Public;
using Kafka.Public.Loggers;

namespace CDC_PoC.Services;

public class KafkaConsumer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly ClusterClient _cluster;
    // private IDocumentTenantIdInjector _documentTenantIdInjector = null!;
    private IElasticCudService _elasticService = null!;

    public KafkaConsumer(IServiceScopeFactory scopeFactory,
        ILogger<KafkaConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _cluster = CreateClusterClient();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        // _documentTenantIdInjector = scope.ServiceProvider.GetRequiredService<IDocumentTenantIdInjector>();
        _elasticService = scope.ServiceProvider.GetRequiredService<IElasticCudService>();

        Initialize();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cluster.Dispose();
        return Task.CompletedTask;
    }

    private static ClusterClient CreateClusterClient()
    {
        return new ClusterClient(new Configuration()
        {
            Seeds = "localhost:9092"
        }, new ConsoleLogger());
    }

    private void Initialize()
    {
        _cluster.MessageReceived += HandleMessage;

        _cluster.ConsumeFromLatest("dev-sqlcust-601.devtung.dbo.dm_location");
        _cluster.ConsumeFromLatest("dev-sqlcust-601.devtung.dbo.dm_employee");
        _cluster.ConsumeFromLatest("dev-sqlcust-601.devtung.dbo.AccountBase");
        _cluster.ConsumeFromLatest("dev-sqlcust-601.appledev.dbo.dm_location");
        _cluster.ConsumeFromLatest("dev-sqlcust-601.appledev.dbo.dm_employee");
    }

    private void HandleMessage(RawKafkaRecord record)
    {
        if (record.Value is byte[] byteArray)
        {
            var message = Encoding.UTF8.GetString(byteArray);
            var response = JsonSerializer.Deserialize<KafkaResponseBody>(message);

            if (response?.Payload is null) return;
            
            _logger.LogInformation($"Consumed a message:\n {response}");
            _elasticService.HandleChanges(response.Payload).GetAwaiter().GetResult();
        }
        else
        {
            _logger.LogInformation("Received a message that is not a byte array");
        }
    }
}