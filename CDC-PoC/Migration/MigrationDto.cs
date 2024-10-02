using System.Text.Json;

namespace CDC_PoC.Migration;

public class MigrationDto
{
    public required string Type { get; set; }
    public required IEnumerable<JsonElement> SearchData { get; set; }
}