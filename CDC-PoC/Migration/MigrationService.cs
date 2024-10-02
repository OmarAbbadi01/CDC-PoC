using CDC_PoC.Config;
using CDC_PoC.Elastic;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;

namespace CDC_PoC.Migration;

public class MigrationService : IMigrationService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ILogger<MigrationService> _logger;
    private readonly AppConfig _appConfig;

    public MigrationService(ElasticsearchClient elasticClient, ILogger<MigrationService> logger, AppConfig appConfig)
    {
        _elasticClient = elasticClient;
        _logger = logger;
        _appConfig = appConfig;
    }

    public async Task MigrateDataAsync(int tenantId, MigrationDto dto)
    {
        var documents = dto.MapToElasticsearchDocuments(tenantId);
        
        var operations = documents.Select(doc => new BulkIndexOperation<ElasticsearchDocument>(doc)
        {
            Routing = tenantId,
            Index = _appConfig.ElasticsearchConfiguration.IndexName
        });
        var bulkRequest = new BulkRequest()
        {
            Operations = new BulkOperationsCollection(operations)
        };
        
        var response = await _elasticClient.BulkAsync(bulkRequest);

        if (response.Errors)
        {
            _logger.LogError("Bulk indexing had errors");
            foreach (var itemWithError in response.ItemsWithErrors)
            {
                _logger.LogError("Failed document at index: {Index}, Reason: {Error}", itemWithError.Index, itemWithError.Error?.Reason);
            }
        }
    }
}