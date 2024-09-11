using CDC_PoC.Constants;
using Nest;
using Newtonsoft.Json;

namespace CDC_PoC.Models.ElasticDocuments;

public class Location : IElasticDocument
{
    [JsonProperty(FieldsNames.LocationFields.LocationIdField)]
    [PropertyName(FieldsNames.LocationFields.LocationIdField)]
    public Guid LocationId { get; set; }

    [JsonProperty(FieldsNames.LocationFields.NameField)]
    [PropertyName(FieldsNames.LocationFields.NameField)]
    public string? Name { get; set; }
    
    [JsonProperty(FieldsNames.LocationFields.CityField)]
    [PropertyName(FieldsNames.LocationFields.CityField)]
    public string? City { get; set; }
    
    [JsonProperty(FieldsNames.LocationFields.StateField)]
    [PropertyName(FieldsNames.LocationFields.StateField)]
    public string? State { get; set; }

    [JsonProperty(FieldsNames.TenantIdField)]
    [PropertyName(FieldsNames.TenantIdField)]
    public string TenantId { get; set; }

    public string GetId()
    {
        return LocationId.ToString();
    }
    
    public string GetIdFieldName()
    {
        return FieldsNames.LocationFields.LocationIdField;
    }
}