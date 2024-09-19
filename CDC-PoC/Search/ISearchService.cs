namespace CDC_PoC.Search;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(int tenantId, string searchTerm, bool exactMatch);
}