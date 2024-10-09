using Microsoft.AspNetCore.Mvc;
using WebApplication1.CustomAttributes;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[action]")]
[CustomAuthorize]
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
        try
        {
            return await _newsService.GetNewsByDates(from, to);
        }
        catch (Exception e)
        {
            return BadRequest($"Error while performing request \n Error message: {e.Message}");
        }
    }

    [ActionName("topTen")]
    [HttpGet]
    public async Task<ActionResult<Dictionary<string, int>>> GetTopTenWords()
    {
        try
        {
            return Ok(await _newsService.GetMostUsedWords());
        }
        catch (Exception e)
        {
            return BadRequest($"Error while performing request \n Error message: {e.Message}");
        }
    }

    [ActionName("search")]
    [HttpGet]
    public async Task<ActionResult<List<News>>> GetNewsByWord([FromQuery] string word)
    {
        try
        {
            return Ok(await _newsService.GetNewsByWord(word));
        }
        catch (Exception e)
        {
            return BadRequest($"Error while performing request \n Error message: {e.Message}");
        }
    }
}