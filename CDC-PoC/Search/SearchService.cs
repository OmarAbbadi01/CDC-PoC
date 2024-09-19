using CDC_PoC.Config;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
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

    public async Task<SearchResult> SearchAsync(int tenantId, string searchTerm, bool exactMatch)
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
                .Size(10)
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
                        .Fields(_appConfig.Value.SearchableFields.GetAllSearchableFields())
                        .Fuzziness(new Fuzziness("AUTO"))
                        .Operator(Operator.And)
                    )
                )
                .Size(10)
                .From(0)
            );
        }

        return new SearchResult
        {
            Documents = searchResponse.Documents,
            size = searchResponse.Documents.Count
        };
    }
}