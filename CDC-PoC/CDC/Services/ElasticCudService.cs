using CDC_PoC.CDC.Models;
using CDC_PoC.Config;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace CDC_PoC.CDC.Services;

public class ElasticCudService : IElasticCudService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ICustomerService _customerService;
    private readonly ILogger<ElasticCudService> _logger;
    private readonly IOptions<AppConfig> _appConfig;

    public ElasticCudService(ElasticsearchClient elasticClient,
        ILogger<ElasticCudService> logger,
        ICustomerService customerService, IOptions<AppConfig> appConfig)
    {
        _elasticClient = elasticClient;
        _logger = logger;
        _customerService = customerService;
        _appConfig = appConfig;
    }

    public async Task HandleChanges(Payload payload)
    {
        switch (payload.Operation.ToLower())
        {
            case "c":
                await HandleCreate(payload);
                break;
            case "u":
                await HandleUpdate(payload);
                break;
            case "d":
                await HandleDelete(payload);
                break;
        }
    }

    private async Task HandleCreate(Payload payload)
    {
        if (payload.After is null)
        {
            throw new Exception("Document is null.");
        }

        var tenantId = await GetCustomerId(payload);
        var elasticDoc = new ElasticsearchDocument()
        {
            TenantId = tenantId,
            Type = payload.Source.Table,
            Value = payload.After.Value
        };

        var indexResponse = await _elasticClient.IndexAsync(elasticDoc, i => i
                .Index(_appConfig.Value.ElasticsearchConfiguration.IndexName)
                .Routing(elasticDoc.TenantId)
                .Id(null) // To Use Elastic's ID Auto Generation
        );

        if (!indexResponse.IsValidResponse)
        {
            _logger.LogError($"Error saving the new document. {indexResponse.ElasticsearchServerError}");
        }
    }

    private async Task HandleUpdate(Payload payload)
    {
        if (payload.Before is null || payload.After is null)
        {
            throw new Exception("Document is null");
        }

        var tenantId = await GetCustomerId(payload);
        var idFieldName = GetIdFieldName(payload.Source.Table);
        var idFieldValue = payload.Before.Value.GetProperty(idFieldName).ToString();

        // delete old document
        var deleteResponse = await _elasticClient.DeleteByQueryAsync(new DeleteByQueryRequest(_appConfig.Value.ElasticsearchConfiguration.IndexName)
        {
            Routing = tenantId,
            Query = new MatchQuery($"value.{idFieldName}"!)
            {
                Query = idFieldValue
            }
        });

        if (!deleteResponse.IsValidResponse)
        {
            _logger.LogError($"Error deleting the document. {deleteResponse.ElasticsearchServerError}");
        }
        
        // Insert the new one
        var elasticDoc = new ElasticsearchDocument()
        {
            TenantId = tenantId,
            Type = payload.Source.Table,
            Value = payload.After.Value
        };

        var indexResponse = await _elasticClient.IndexAsync(elasticDoc, i => i
                .Index(_appConfig.Value.ElasticsearchConfiguration.IndexName)
                .Routing(elasticDoc.TenantId)
                .Id(null) // To Use Elastic's ID Auto Generation
        );

        if (!indexResponse.IsValidResponse)
        {
            _logger.LogError($"Error indexing the document. {indexResponse.ElasticsearchServerError}");
        }
    }

    private async Task HandleDelete(Payload payload)
    {
        if (payload.Before is null)
        {
            throw new Exception("Document is null");
        }

        var tenantId = await GetCustomerId(payload);
        var idFieldName = GetIdFieldName(payload.Source.Table);
        var idFieldValue = payload.Before.Value.GetProperty(idFieldName).ToString();

        var deleteResponse = await _elasticClient.DeleteByQueryAsync(new DeleteByQueryRequest(_appConfig.Value.ElasticsearchConfiguration.IndexName)
        {
            Routing = tenantId,
            Query = new MatchQuery($"value.{idFieldName}"!)
            {
                Query = idFieldValue
            }
        });

        if (!deleteResponse.IsValidResponse)
        {
            _logger.LogWarning($"Error saving the new document. {deleteResponse.ElasticsearchServerError}");
        }
    }

    private async Task<int> GetCustomerId(Payload payload)
    {
        return await _customerService.GetCustomerIdByDbName(payload.Source.Db);
    }
    
    // TODO: move this out
    private static string GetIdFieldName(string tableName)
    {
        return tableName switch
        {
            "dm_location" => "dm_locationId",
            "dm_employee" => "dm_employeeId",
            "AccountBase" => "AccountId",
            _ => "Id"
        };
    }
}