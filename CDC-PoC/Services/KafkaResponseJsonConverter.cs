using CDC_PoC.Models;
using CDC_PoC.Models.DocumentsModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CDC_PoC.Services;

public class KafkaResponseJsonConverter : JsonConverter<KafkaResponseBody>
{
    public override KafkaResponseBody ReadJson(JsonReader reader, Type objectType, KafkaResponseBody? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var payloadObject = jsonObject["payload"];

        if (payloadObject == null)
        {
            throw new JsonSerializationException("Missing required field: payload");
        }

        PayloadJsonValidator.ValidateRequiredFields((JObject)payloadObject);

        var payload = new Payload
        {
            Operation = payloadObject["op"]!.ToString(),
            Timestamp = payloadObject["ts_ms"]!.Value<long>(),
            Source = payloadObject["source"]!.ToObject<Source>()!
        };

        var tableName = payload.Source.Table;

        // TODO: move this to repository or cache or smth
        var objectTypeToUse = tableName switch
        {
            "dm_location" => typeof(Location),
            "dm_employee" => typeof(Employee),
            _ => throw new JsonSerializationException($"Unknown table type: {tableName}")
        };

        payload.Before = payloadObject["before"]?.ToObject(objectTypeToUse, serializer) as IElasticDocument;
        payload.After = payloadObject["after"]?.ToObject(objectTypeToUse, serializer) as IElasticDocument;

        return new KafkaResponseBody { Payload = payload };
    }

    public override void WriteJson(JsonWriter writer, KafkaResponseBody? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}