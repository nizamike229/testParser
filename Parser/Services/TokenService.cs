using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services;

public static class TokenService
{
    private const string SecretKey = "qwerty188914485asdfghjklzxcv.";

    public static string GenerateToken(string username)
    {
        var encodedUsername = Convert.ToBase64String(Encoding.UTF8.GetBytes(username));
        var tokenPayload = $"{encodedUsername}:{DateTime.UtcNow.Ticks}";
        var tokenSignature = SignToken(tokenPayload);
        return $"{tokenPayload}:{tokenSignature}";
    }

    public static bool IsTokenValid(string token)
    {
        var tokenParts = token.Split(':');
        if (tokenParts.Length != 3)
            return false;

        var tokenPayload = $"{tokenParts[0]}:{tokenParts[1]}";
        var tokenSignature = tokenParts[2];

        var expectedSignature = SignToken(tokenPayload);
        return expectedSignature == tokenSignature;
    }

    private static string SignToken(string tokenPayload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenPayload));
        return Convert.ToBase64String(hash);
    }
}