namespace CDC_PoC.Config;

public class AppConfig
{
    public required CustomerSettingConfiguration CustomerSettingConfiguration { get; set; }
    public required ElasticsearchConfiguration ElasticsearchConfiguration { get; set; }
}

public class CustomerSettingConfiguration
{
    public required string ApiUrl { get; set; }
}

public class ElasticsearchConfiguration
{
    public required string ElasticClientHost { get; set; }
}