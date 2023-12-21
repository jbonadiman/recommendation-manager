using Microsoft.AspNetCore.Mvc;
using RecommendationManager.Application.Interfaces;
using RecommendationManager.Application.Models.Books;

namespace RecommendationManager.API.Controllers;

[ApiController]
public class RecommendationController : ControllerBase
{
    private readonly ILogger<RecommendationController> _logger;
    private readonly IBookService _bookService;

    public RecommendationController(
        ILogger<RecommendationController> logger,
        IBookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    [HttpPost("/")]
    public async Task SaveBundleAsync([FromBody] SaveBundleRequest saveRequest)
    {
        await _bookService.SaveBundleAsync(saveRequest);
    }
}
