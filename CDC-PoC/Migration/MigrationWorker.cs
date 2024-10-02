using System.Text.Json;
using AutoFixture;

namespace CDC_PoC.Migration;

public class MigrationWorker : BackgroundService
{
    private readonly ILogger<MigrationWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFixture _fixture;
    private IMigrationService _migrationService = null!;

    public MigrationWorker(ILogger<MigrationWorker> logger, IServiceScopeFactory scopeFactory, IFixture fixture)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _fixture = fixture;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        using var scope = _scopeFactory.CreateScope();
        _migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Migration Worker running at: {time}", DateTimeOffset.Now);
            var accounts = _fixture.CreateMany<AccountBase>(10000);
            var dto = CreateMigrationDto(accounts);
            var tenantId = 17;
            
            await _migrationService.MigrateDataAsync(tenantId, dto);
        }
    }

    private MigrationDto CreateMigrationDto(IEnumerable<AccountBase> accounts)
    {
        var list = accounts.Select(account =>
            {
                var jsonString = JsonSerializer.Serialize(account);
                var jsonDocument = JsonDocument.Parse(jsonString);
                var jsonElement = jsonDocument.RootElement;
                return jsonElement;
            })
            .AsEnumerable();
        
        return new MigrationDto
        {
            Type = "AccountBase",
            SearchData = list
        };
    }
}