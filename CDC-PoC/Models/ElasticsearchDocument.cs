namespace CDC_PoC.Models;

public class ElasticsearchDocument
{
    public string? Id { get; set; }

    public int TenantId { get; set; }
    
    public string Type { get; set; }
    
    public object Value { get; set; }
}