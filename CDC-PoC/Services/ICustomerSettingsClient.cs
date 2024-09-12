using CDC_PoC.Models;
using Refit;

namespace CDC_PoC.Services;

[Headers("Content-type: application/json")]
public interface ICustomerSettingsClient
{
    [Get("/Setting/{setting}")]
    Task<CustomerSettingValueResponse> GetCustomerSetting(string setting, [Header("X-R365-Customer-instance")] string customerInstanceName);
}