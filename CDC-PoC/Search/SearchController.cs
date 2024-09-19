using Microsoft.AspNetCore.Mvc;

namespace CDC_PoC.Search;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{

    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromHeader(Name = "X-Tenant-ID")] int tenantId, 
        [FromQuery(Name = "term")] string term,
        [FromQuery(Name = "exact-match")] bool exactMatch)
    {
        var docs = await _searchService.SearchAsync(tenantId, term, exactMatch);
        return Ok(docs);
    }
    
}