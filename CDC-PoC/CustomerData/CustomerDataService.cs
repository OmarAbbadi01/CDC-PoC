using CDC_PoC.Config;
using CDC_PoC.Elastic;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace CDC_PoC.CustomerData;

public class CustomerDataService : ICustomerDataService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly IOptions<AppConfig> _appConfig;
    private readonly ILogger<CustomerDataService> _logger;

    public CustomerDataService(ElasticsearchClient elasticClient, IOptions<AppConfig> appConfig, ILogger<CustomerDataService> logger)
    {
        _elasticClient = elasticClient;
        _appConfig = appConfig;
        _logger = logger;
    }

    public async Task<SearchResult> SearchAsync(int tenantId, string searchTerm, bool exactMatch, int size)
    {
        SearchResponse<object> searchResponse;
        if (exactMatch)
        {
            searchResponse = await  _elasticClient.SearchAsync<object>(s => s
                .Index(_appConfig.Value.ElasticsearchConfiguration.IndexName)
                .Routing(tenantId)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(searchTerm)
                        .Fields(_appConfig.Value.SearchableFields.GetAllSearchableFields())
                        .Type(TextQueryType.Phrase)
                    )
                )
                .Size(size)
                .From(0)
            );
        }
        else
        {
            searchResponse = await  _elasticClient.SearchAsync<object>(s => s
                .Index(_appConfig.Value.ElasticsearchConfiguration.IndexName)
                .Routing(tenantId)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(searchTerm)
                        .Fields("*") // TODO: should be configurable
                        .Fuzziness(new Fuzziness("AUTO"))
                        .Operator(Operator.And)
                    )
                )
                .Size(size)
                .From(0)
            );
        }

        return new SearchResult
        {
            Documents = searchResponse.Documents,
            Size = searchResponse.Documents.Count
        };
    }
    
    public async Task MigrateDataAsync(int tenantId, MigrationDto dto)
    {
        var documents = dto.MapToElasticsearchDocuments(tenantId);
        
        var operations = documents.Select(doc => new BulkIndexOperation<ElasticsearchDocument>(doc)
        {
            Routing = tenantId,
            Index = _appConfig.Value.ElasticsearchConfiguration.IndexName
        });
        var bulkRequest = new BulkRequest()
        {
            Operations = new BulkOperationsCollection(operations),
            Refresh = Refresh.True
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