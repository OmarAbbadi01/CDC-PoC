namespace CDC_PoC.Search;

public interface ISearchService
{
    Task<IEnumerable<object>> SearchAsync(int tenantId, string searchTerm);
}