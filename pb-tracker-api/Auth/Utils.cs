using Microsoft.AspNetCore.Http;
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

