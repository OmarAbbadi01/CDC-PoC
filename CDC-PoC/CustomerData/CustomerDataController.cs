using Microsoft.AspNetCore.Mvc;
using R365.FireAndForget;

namespace CDC_PoC.CustomerData;

[ApiController]
[Route("customer-data")]
public class CustomerDataController : ControllerBase
{
    private readonly ICustomerDataService _customerDataService;
    private readonly IForgettable<ICustomerDataService> _forgettableCustomerDataService;

    public CustomerDataController(ICustomerDataService customerDataService, IForgettable<ICustomerDataService> forgettableCustomerDataService)
    {
        _customerDataService = customerDataService;
        _forgettableCustomerDataService = forgettableCustomerDataService;
    }

    // TODO: remove tenant id from response body
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromHeader(Name = "X-Tenant-ID")] int tenantId, 
        [FromQuery(Name = "term")] string term,
        [FromQuery(Name = "exact-match")] bool exactMatch,
        [FromQuery(Name = "size")] int size = 10)
    {
        var docs = await _customerDataService.SearchAsync(tenantId, term, exactMatch, size);
        return Ok(docs);
    }
    
    [HttpPost]
    public IActionResult MigrateData([FromHeader(Name = "X-Tenant-ID")] int tenantId, [FromBody] MigrationDto dto)
    {
        _forgettableCustomerDataService.CallAndForgetAsync(async x => await x.MigrateDataAsync(tenantId, dto));
        return Accepted();
    }
    
}