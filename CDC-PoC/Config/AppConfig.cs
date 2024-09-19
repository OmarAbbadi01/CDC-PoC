using System.Text.Json.Serialization;

namespace CDC_PoC.Config;

public class AppConfig
{
    public required CustomerSettingConfiguration CustomerSettingConfiguration { get; set; }
    public required ElasticsearchConfiguration ElasticsearchConfiguration { get; set; }
    public required KafkaConfiguration KafkaConfiguration { get; set; }
    public required SearchableFields SearchableFields { get; set; }
}

public class CustomerSettingConfiguration
{
    public required string ApiUrl { get; set; }
}

public class ElasticsearchConfiguration
{
    public required string ElasticClientHost { get; set; }
    public required string IndexName { get; set; }
}

public class KafkaConfiguration
{
    public required string BootstrapServers { get; set; }
    public required string GroupId { get; set; }
    public required IEnumerable<string> Topics { get; set; }
}

public class SearchableFields
{
    public required List<string> Location { get; set; }
    
    public required List<string> Employee { get; set; }

    // TODO: Move this to DB and make it cachable
    public string[] GetAllSearchableFields()
    {
        return Location
            .Select(l => $"value.{l}")
            .Concat(Employee
                .Select(e => $"value.{e}"))
            .ToArray();
    }
}