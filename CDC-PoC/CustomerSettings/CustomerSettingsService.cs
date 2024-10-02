using R365.CustomerSettings;

namespace CDC_PoC.CustomerSettings;

public class CustomerSettingsService : ICustomerSettingsService
{
    private readonly ICustomerSettingsClient _customerSettingsClient;

    public CustomerSettingsService(ICustomerSettingsClient customerSettingsClient)
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