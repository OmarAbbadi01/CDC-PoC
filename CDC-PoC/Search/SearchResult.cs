namespace CDC_PoC.Search;

public class SearchResult
{
    public int size { get; set; }
    public IEnumerable<object> Documents { get; set; }
}