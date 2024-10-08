using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[action]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [ActionName("posts")]
    [HttpGet]
    public async Task<ActionResult<List<News>>> GetByDates([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return await _newsService.GetNewsByDates(from, to);
    }
}