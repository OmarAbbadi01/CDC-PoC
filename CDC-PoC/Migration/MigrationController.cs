using Microsoft.AspNetCore.Mvc;
using R365.FireAndForget;

namespace CDC_PoC.Migration;

[ApiController]
[Route("customer-data")]
public class MigrationController : ControllerBase
{
    private readonly IForgettable<IMigrationService> _migrationService;

    public MigrationController(IForgettable<IMigrationService> migrationService)
    {
        _migrationService = migrationService;
    }

    [HttpPost]
    public IActionResult MigrateData([FromHeader(Name = "X-Tenant-ID")] int tenantId, [FromBody] MigrationDto dto)
    {
        _migrationService.CallAndForgetAsync(async x => await x.MigrateDataAsync(tenantId, dto));
        return Ok();
    }
}