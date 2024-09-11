using System.Text;
using CDC_PoC.Models;
using Kafka.Public;
using Kafka.Public.Loggers;
using Newtonsoft.Json;

namespace CDC_PoC.Services;

public class KafkaConsumer : IHostedService
{
    private readonly IElasticCudService _elasticService;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly ClusterClient _cluster;

    public KafkaConsumer(ILogger<KafkaConsumer> logger, IElasticCudService elasticService)
    {
        _logger = logger;
        _elasticService = elasticService;
        _cluster = CreateClusterClient();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
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
    }

    private void HandleMessage(RawKafkaRecord record)
    {
        if (record.Value is byte[] byteArray)
        {
            var message = Encoding.UTF8.GetString(byteArray);
            var response = JsonConvert.DeserializeObject<KafkaResponseBody>(message);
            
            if (response?.Payload is null) return;
            
            _logger.LogInformation($"Consumed a message:\n {response.Payload}");
            _elasticService.HandleChanges(response.Payload);
        }
        else
        {
            _logger.LogWarning("Received a message that is not a byte array");
        }
    }
}