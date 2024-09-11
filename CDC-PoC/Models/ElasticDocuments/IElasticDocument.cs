namespace CDC_PoC.Models.ElasticDocuments;

public interface IElasticDocument
{
    public string TenantId { get; set; }   
    string GetId();
    string GetIdFieldName();
}