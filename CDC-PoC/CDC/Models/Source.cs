using System.Text.Json.Serialization;

namespace CDC_PoC.CDC.Models;

public class Source
{
    [JsonPropertyName("db")]
    public required string Db { get; set; }
    
    [JsonPropertyName("schema")]
    public required string Schema { get; set; }
    
    [JsonPropertyName("table")]
    public required string Table { get; set; }
}