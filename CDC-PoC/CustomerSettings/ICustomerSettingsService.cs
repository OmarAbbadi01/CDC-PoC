namespace CDC_PoC.CustomerSettings;

public interface ICustomerSettingsService
{
    Task<int> GetCustomerIdByDbName(string databaseName);
}