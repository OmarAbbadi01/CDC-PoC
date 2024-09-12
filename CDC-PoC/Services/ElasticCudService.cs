using CDC_PoC.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace CDC_PoC.Services;

public class ElasticCudService : IElasticCudService
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly ICustomerService _customerService;
    private readonly ILogger<ElasticCudService> _logger;

    public ElasticCudService(ElasticsearchClient elasticClient,
        ILogger<ElasticCudService> logger,
        ICustomerService customerService)
    {
        _elasticClient = elasticClient;
        _logger = logger;
        _customerService = customerService;
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
            CreatedOn = DateTime.Now,
            ModifiedOn = DateTime.Now,
            Value = payload.After.Value
        };

        var indexResponse = await _elasticClient.IndexAsync(elasticDoc, i => i
                .Index("index2")
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

        var updateResponse = await _elasticClient.UpdateByQueryAsync(new UpdateByQueryRequest("index2")
        {
            Routing = tenantId,
            Query = new MatchQuery($"value.{idFieldName}"!)
            {
                Query = idFieldValue
            },
            Script = new Script
            {
                Source = @"
                    ctx._source.value = params.newValue;
                    ctx._source.modifiedOn = params.modifiedOn;
                ",
                Params = new Dictionary<string, object>
                {
                    { "newValue", payload.After.Value },
                    { "modifiedOn", DateTime.Now }
                }
            }
        });

        if (!updateResponse.IsValidResponse)
        {
            _logger.LogWarning($"Error updating the document. {updateResponse.ElasticsearchServerError}");
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

        var deleteResponse = await _elasticClient.DeleteByQueryAsync(new DeleteByQueryRequest("index2")
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