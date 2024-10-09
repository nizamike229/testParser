using Microsoft.AspNetCore.Mvc;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[action]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpPost]
    [ActionName("login")]
    public async Task<ActionResult<string>> Login([FromBody] User user)
    {
        try
        {
            var token = await _userService.LoginAsync(user);
            HttpContext.Session.SetString("auth_token", token);
            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest($"Error while performing request \n Error message: {e.Message}");
        }
    }


    [ActionName("register")]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] User user)
    {
        try
        {
            await _userService.RegisterAsync(user);
            return Ok("User registered successfully!");
        }
        catch (Exception e)
        {
            return BadRequest($"Error while performing request \n Error message: {e.Message}");
        }
    }
}