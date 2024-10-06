using System.Text.Json;

namespace CDC_PoC.CustomerData;

public class MigrationDto
{
    public required string Type { get; set; }
    public required IEnumerable<JsonElement> SearchData { get; set; }
}