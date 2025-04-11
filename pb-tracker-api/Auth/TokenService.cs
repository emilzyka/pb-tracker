using Microsoft.Extensions.Configuration;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Auth;

public interface ITokenService
{
    Token GenerateWebToken(string user, Guid salt);
    void ValidateWebToken(Token token, Guid salt);
}

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;

    public Token GenerateWebToken(string user, Guid salt)
        => GenerateToken(
            _config["SERVICE_TOKEN_KEY"].ToOption(),
            _config["SERVICE_TOKEN_DURATION_SEC"].ToOption(),
            user,
            salt);

    public void ValidateWebToken(Token token, Guid salt)
        => ValidateTokenSignAndExp(
            _config["SERVICE_TOKEN_KEY"].ToOption(),
            token,
            salt);

    private Token GenerateToken(Option<string> key, Option<string> durationSec, string ident, Guid salt)
    {

        byte[] tokenKey = key.Match(
            some => Convert.FromBase64String(some),
            () => throw new InvalidDataException("Token key not found"));

        int duration = durationSec.Match(
            some => int.Parse(some),
            () => throw new InvalidDataException("Token duration not found"));

        string exp = DateTime.UtcNow.AddSeconds(duration).ToString("o");
        string sign = TokenSignIntoB64U(ident, exp, salt, tokenKey);

        return Token.Create(ident, exp, sign);
    }

    private void ValidateTokenSignAndExp(Option<string> key, Token token, Guid salt)
    {
        byte[] tokenKey = key.Match(
             some => Convert.FromBase64String(some),
            () => throw new InvalidDataException("Token key not found"));

        string expectedSign = TokenSignIntoB64U(token.Ident, token.Exp, salt, tokenKey);

        if (expectedSign != token.SignB64U)
            throw new Exception("Token signature does not match");

        if (!DateTime.TryParse(token.Exp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expTime))
            throw new Exception("Invalid expiration format");

        if (DateTime.UtcNow > expTime)
            throw new Exception("Token expired");
    }

    private string TokenSignIntoB64U(string ident, string exp, Guid salt, byte[] key)
    {
        var content = $"{Base64UrlEncode(ident)}.{Base64UrlEncode(exp)}";
        return EncryptIntoB64U(key, content, salt.ToString());
    }

    private string EncryptIntoB64U(byte[] key, string content, string salt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(key);
        var combined = System.Text.Encoding.UTF8.GetBytes(content + salt);
        var hash = hmac.ComputeHash(combined);
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(string value)
    {
        return Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(value));
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
