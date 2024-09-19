using CDC_PoC.Config;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace CDC_PoC.Search;

public class SearchService : ISearchService
{
    private readonly IOptions<AppConfig> _appConfig;
    private readonly ElasticsearchClient _elasticClient;

    public SearchService(ElasticsearchClient elasticClient, IOptions<AppConfig> appConfig)
    {
        _elasticClient = elasticClient;
        _appConfig = appConfig;
    }

    public async Task<IEnumerable<object>> SearchAsync(int tenantId, string searchTerm)
    {
        var result = await _elasticClient.SearchAsync<object>(s => s
            .Index(_appConfig.Value.ElasticsearchConfiguration.IndexName)
            .Routing(tenantId.ToString())
            .Query(q => q.QueryString(qs => qs
                .Query($"*{searchTerm}*")))
        );

        return result.Documents;
    }
}