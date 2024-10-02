using System.Text.Json;
using R365.Messaging.Contracts.Attributes;

[assembly: EventStreamNamespaceAlias("Events")]

namespace CDC_PoC.EntitySync;

[EventStreamContract("entitysyncdto")]
public class EntitySyncDto
{
    [EventStreamPartitionKey(ordinal: 0)] 
    public int TenantId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; }
    public object Entity { get; set; }
    public string Operation { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }
}