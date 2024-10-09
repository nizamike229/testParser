using WebApplication1.Models;

namespace WebApplication1.Interfaces;

public interface IUserService
{
    Task<string> LoginAsync(User user);
    Task RegisterAsync(User user); 
}