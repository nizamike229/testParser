using System.Security.Cryptography;
using System.Text;
using Dapper;
using Npgsql;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly NpgsqlConnection _connection;

    public UserService(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<string> LoginAsync(User user)
    {
        var isUserValid = (await _connection.QueryAsync<bool>("select check_user_valid(@inputUsername,@inputPassword)",
            new
            {
                inputUsername = user.Username,
                inputPassword = HashPassword(user.Password)
            })).FirstOrDefault();

        if (!isUserValid) throw new Exception("Invalid username or password");

        var token = TokenService.GenerateToken(user.Username);
        return token;
    }

    public async Task RegisterAsync(User user)
    {
        var isUserExist = (await _connection.QueryAsync("select * from users where username = @inputLogin", new
        {
            inputLogin = user.Username
        })).Any();
        if (!isUserExist)
        {
            await _connection.ExecuteAsync("call create_user(@insertUsername,@insertPassword)", new
            {
                insertUsername = user.Username,
                insertPassword = HashPassword(user.Password)
            });
        }
        else
        {
            throw new Exception("User already exists");
        }
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}