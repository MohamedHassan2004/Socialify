using Microsoft.AspNetCore.Mvc;
using Socialify.Application.DTOs.Search;
using Socialify.Application.Services_Interfaces;

public class SearchController : Controller
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return View(new SearchResultDto());

        ViewBag.Query = query;
        var result = await _searchService.SearchAsync(query);

        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "An error occurred while searching. Please try again.";
            return View(new SearchResultDto());
        }
        return View(result.Data);
    }
}