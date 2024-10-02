namespace CDC_PoC.Migration;

public interface IMigrationService
{
    Task MigrateDataAsync(int tenantId, MigrationDto dto);
}