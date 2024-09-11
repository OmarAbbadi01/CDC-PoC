using CDC_PoC.Models;
using R365.CustomerSettings;

namespace CDC_PoC.Services;

public class DocumentTenantIdInjector : IDocumentTenantIdInjector
{
    private readonly ICustomerSettingRestServices _customerSettingRestServices;

    public DocumentTenantIdInjector(ICustomerSettingRestServices customerSettingRestServices)
    {
        _customerSettingRestServices = customerSettingRestServices;
    }

    public async Task<Payload> InjectTenantId(Payload payload)
    {
        var databaseName = payload.Source.Db;

        if (payload.Before is not null)
        {
            payload.Before.TenantId = await GetCustomerIdByDbName(databaseName);
        }

        if (payload.After is not null)
        {
            payload.After.TenantId = await GetCustomerIdByDbName(databaseName);
        }

        return payload;
    }

    private async Task<string> GetCustomerIdByDbName(string databaseName)
    {
        var response = await _customerSettingRestServices.GetCustomerSetting(SettingId.CustomerId.ToString(), databaseName);
        return response.Value;
    }
}