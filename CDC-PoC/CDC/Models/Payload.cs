using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDC_PoC.CDC.Models;

public class Payload
{
    [JsonPropertyName("before")]
    public JsonElement? Before { get; set; }

    [JsonPropertyName("after")]
    public JsonElement? After { get; set; }

    [JsonPropertyName("source")]
    public required Source Source { get; set; }

    [JsonPropertyName("op")]
    public required string Operation { get; set; }

    [JsonPropertyName("ts_ms")]
    public required long Timestamp { get; set; }
}
