namespace CDC_PoC.Services;

public interface ICustomerService
{
    Task<int> GetCustomerIdByDbName(string databaseName);
}