namespace CDC_PoC.Models.DocumentsModels;

public interface IElasticDocument
{
    string GetId();
    string GetIdFieldName();
}