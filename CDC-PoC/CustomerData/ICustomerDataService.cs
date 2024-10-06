namespace CDC_PoC.CustomerData;

public interface ICustomerDataService
{
    Task<SearchResult> SearchAsync(int tenantId, string searchTerm, bool exactMatch, int size);
    
    Task MigrateDataAsync(int tenantId, MigrationDto dto);
}