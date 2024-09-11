using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CDC_PoC.Services;

public static class PayloadJsonValidator
{
    private static readonly List<string> PayloadRequiredFields = ["op", "ts_ms", "source"];
    private static readonly List<string> SourceRequiredFields = ["db", "schema", "table"];

    public static void ValidateRequiredFields(JObject jsonObject)
    {
        foreach (var field in PayloadRequiredFields)
        {
            if (jsonObject[field] is null)
            {
                throw new JsonSerializationException($"Missing required field: {field}");
            }
        }

        var source = jsonObject["source"];

        if (source is null)
        {
            throw new JsonSerializationException("Missing required field: source");
        }

        foreach (var field in SourceRequiredFields)
        {
            if (source[field] is null)
            {
                throw new JsonSerializationException($"Missing required field: source.{field}");
            }
        }
    }
}