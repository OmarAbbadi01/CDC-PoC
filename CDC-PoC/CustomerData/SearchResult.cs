namespace CDC_PoC.CustomerData;

public class SearchResult
{
    public required int Size { get; set; }
    public required IEnumerable<object> Documents { get; set; }
}