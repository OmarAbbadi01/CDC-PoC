using System.Text.Json;
using System.Text.Json.Serialization;

namespace CDC_PoC.Models;

public class KafkaResponseBody
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    [JsonPropertyName("payload")] 
    public required Payload Payload { get; set; }

    // For better visualization
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, Options);
    }
}