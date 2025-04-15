using Microsoft.AspNetCore.Http;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Auth;

public static class Utils
{
    public static void SetTokenCookie(HttpResponse response, Token token)
    {
        response.Cookies.Append(
            "auth-token",
            token.ToString(),
            new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Parse(token.Exp)
            });
    }

    public static Task<Result<Token, IError>> TokenTryParse(string raw)
    {
        // "username.exp.sign"

        string[] parts = raw.Split('.');

        if (parts.Length != 4)
        {
            return Task.FromResult(Result<Token, IError>.Err(new MissingAuth("Auth failed", nameof(TokenTryParse))));
        }

        var (username, exp, sign) = (parts[0], parts[1], parts[2]);

        return Task.FromResult(Result<Token, IError>.Ok(Token.Create(username, exp, sign)));
    }


    public static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static byte[] Base64UrlDecode(string base64Url)
    {
        string padded = base64Url
            .Replace('-', '+')
            .Replace('_', '/');

        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }
        return Convert.FromBase64String(padded);
    }
}

