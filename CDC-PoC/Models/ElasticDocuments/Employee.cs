using CDC_PoC.Constants;
using Nest;
using Newtonsoft.Json;

namespace CDC_PoC.Models.ElasticDocuments;

public class Employee : IElasticDocument
{
    [JsonProperty(FieldsNames.EmployeeFields.EmployeeId)]
    [PropertyName(FieldsNames.EmployeeFields.EmployeeId)]
    public Guid EmployeeId { get; set; }
    
    [JsonProperty(FieldsNames.EmployeeFields.EmployeeFullName)]
    [PropertyName(FieldsNames.EmployeeFields.EmployeeFullName)]
    public string? FullName { get; set; }

    [JsonProperty(FieldsNames.EmployeeFields.EmployeeCity)]
    [PropertyName(FieldsNames.EmployeeFields.EmployeeCity)]
    public string? City { get; set; }
    
    [JsonProperty(FieldsNames.EmployeeFields.EmployeeState)]
    [PropertyName(FieldsNames.EmployeeFields.EmployeeState)]
    public string? State { get; set; }
    
    [JsonProperty(FieldsNames.TenantIdField)]
    [PropertyName(FieldsNames.TenantIdField)]
    public string TenantId { get; set; }

    public string GetId()
    {
        return EmployeeId.ToString();
    }
    
    public string GetIdFieldName()
    {
        return FieldsNames.EmployeeFields.EmployeeId;
    }
}