using CDC_PoC.Models.ElasticDocuments;
using Newtonsoft.Json;

namespace CDC_PoC.Models;

public class Payload
{
    [JsonProperty("before")]
    public IElasticDocument? Before { get; set; }

    [JsonProperty("after")]
    public IElasticDocument? After { get; set; }

    [JsonProperty("source")]
    public required Source Source { get; set; }

    [JsonProperty("op")]
    public required string Operation { get; set; }

    [JsonProperty("ts_ms")]
    public required long Timestamp { get; set; }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
