using Microsoft.Azure.Functions.Worker.Http;

namespace pb_tracker_api.Auth;

public static class Cookie
{
    public static void SetTokenCookie(HttpResponseData response, string token)
    {
        var cookieValue = $"auth-token={token}; Path=/; HttpOnly";
        response.Headers.Add("Set-Cookie", cookieValue);
    }
}

