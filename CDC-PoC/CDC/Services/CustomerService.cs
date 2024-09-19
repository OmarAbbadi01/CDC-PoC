using R365.CustomerSettings;

namespace CDC_PoC.CDC.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerSettingsClient _customerSettingsClient;

    public CustomerService(ICustomerSettingsClient customerSettingsClient)
    {
        _customerSettingsClient = customerSettingsClient;
    }

    //TODO: Needs caching
    public async Task<int> GetCustomerIdByDbName(string databaseName)
    {
        var response = await _customerSettingsClient.GetCustomerSetting(SettingId.CustomerId.ToString(), databaseName);
        if (int.TryParse(response.Value, out var id))
        {
            return id;
        }

        throw new Exception("No such customer!");
    }
}