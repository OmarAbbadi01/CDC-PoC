using System.Text.Json;
using CDC_PoC.Config;
using CDC_PoC.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CDC_PoC.Services;

public class KafkaConsumer : IHostedService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IOptions<AppConfig> _appConfig;
    private readonly IServiceScopeFactory _scopeFactory;
    private IElasticCudService _elasticCudService = null!;

    public KafkaConsumer(ILogger<KafkaConsumer> logger,
        IOptions<AppConfig> appConfig,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _appConfig = appConfig;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _appConfig.Value.KafkaConfiguration.BootstrapServers,
            GroupId = _appConfig.Value.KafkaConfiguration.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        using var scope = _scopeFactory.CreateScope();
        _elasticCudService = scope.ServiceProvider.GetRequiredService<IElasticCudService>();

        consumer.Subscribe(_appConfig.Value.KafkaConfiguration.Topics);
        
        await ConsumeMessages(consumer, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ConsumeMessages(IConsumer<Ignore, string> consumer, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(cancellationToken);

                if (consumeResult.Message.Value is null) continue;

                var response = JsonSerializer.Deserialize<KafkaResponseBody>(consumeResult.Message.Value);
                _logger.LogInformation($"Consumed a message:\n {response}");

                if (response?.Payload is null) continue;
                
                await _elasticCudService.HandleChanges(response.Payload);

                consumer.Commit(consumeResult);
            }
            catch (ConsumeException e)
            {
                _logger.LogError($"Error consuming message: {e.Error.Reason}");
            }
        }
    }
}