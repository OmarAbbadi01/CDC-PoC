using CDC_PoC.Elastic;
using Riok.Mapperly.Abstractions;

namespace CDC_PoC.Migration;

[Mapper]
public static partial class MigrationDtoMapper
{
    public static List<ElasticsearchDocument> MapToElasticsearchDocuments(this MigrationDto source, int tenantId)
    {
        return source.SearchData
            .Select(item => MapToDocument(item, source.Type, tenantId))
            .ToList();
    }

    private static ElasticsearchDocument MapToDocument(object value, string type, int tenantId)
    {
        return new ElasticsearchDocument
        {
            TenantId = tenantId,
            Type = type,
            Value = value
        };
    }
}