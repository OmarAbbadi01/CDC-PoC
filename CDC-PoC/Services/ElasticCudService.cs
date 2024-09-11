using CDC_PoC.Models;
using Nest;

namespace CDC_PoC.Services;

public class ElasticCudService : IElasticCudService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticCudService> _logger;

    public ElasticCudService(IElasticClient elasticClient, ILogger<ElasticCudService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
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
        var newDocument = payload.After;

        if (newDocument is null)
        {
            throw new Exception("Document is null.");
        }

        var indexResponse = await _elasticClient.IndexAsync<object>(newDocument, i => i
            .Index("index1")
            .Routing(newDocument.TenantId));

        if (!indexResponse.IsValid)
        {
            _logger.LogError($"Error saving the new document. {indexResponse.ServerError}");
        }
    }

    private async Task HandleUpdate(Payload payload)
    {
        var oldDocument = payload.Before;
        var newDocument = payload.After;

        if (oldDocument is null || newDocument is null)
        {
            throw new Exception("Document is null");
        }

        var updateResponse = await _elasticClient.UpdateByQueryAsync<object>(u => u
            .Index("index1")
            .Routing(oldDocument.TenantId)
            .Query(q => q
                .Match(m => m
                    .Field(oldDocument.GetIdFieldName())
                    .Query(oldDocument.GetId())
                )
            )
            .Script(s => s
                .Source(@"ctx._source = params.newDocument;")
                .Params(p => p
                    .Add("newDocument", newDocument))
            )
        );

        if (!updateResponse.IsValid)
        {
            _logger.LogWarning($"Error updating the document. {updateResponse.ServerError}");
        }
    }

    private async Task HandleDelete(Payload payload)
    {
        var oldDocument = payload.Before;

        if (oldDocument is null)
        {
            throw new Exception("Document is null");
        }

        var indexResponse = await _elasticClient.DeleteByQueryAsync<object>(d => d
            .Index("index1")
            .Routing(oldDocument.TenantId)
            .Query(q => q
                .Match(m => m
                    .Field(oldDocument.GetIdFieldName())
                    .Query(oldDocument.GetId())
                )
            )
        );

        if (!indexResponse.IsValid)
        {
            _logger.LogWarning($"Error saving the new document. {indexResponse.ServerError}");
        }
    }
}