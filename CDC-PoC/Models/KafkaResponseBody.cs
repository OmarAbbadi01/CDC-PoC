using CDC_PoC.Services;
using Newtonsoft.Json;

namespace CDC_PoC.Models;

[JsonConverter(typeof(KafkaResponseJsonConverter))]
public class KafkaResponseBody
{
    [JsonProperty("payload")]
    public required Payload Payload { get; set; }
}