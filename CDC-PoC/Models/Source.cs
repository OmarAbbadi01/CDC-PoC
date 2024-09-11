using Newtonsoft.Json;

namespace CDC_PoC.Models;

public class Source
{
    [JsonProperty("db")]
    public required string Db { get; set; }
    
    [JsonProperty("schema")]
    public required string Schema { get; set; }
    
    [JsonProperty("table")]
    public required string Table { get; set; }
}