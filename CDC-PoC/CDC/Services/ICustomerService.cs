namespace CDC_PoC.CDC.Services;

public interface ICustomerService
{
    Task<int> GetCustomerIdByDbName(string databaseName);
}