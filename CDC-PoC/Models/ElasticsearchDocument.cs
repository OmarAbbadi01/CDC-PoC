using System.Text.Json;

namespace CDC_PoC.Models;

public class ElasticsearchDocument
{
    public string? Id { get; set; }

    public int TenantId { get; set; }
    
    public string Type { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }
    
    public object Value { get; set; }
}