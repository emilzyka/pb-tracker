using System.Net;
using Microsoft.AspNetCore.Http;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Bmc;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Auth;

public interface IAuthResolver
{
    Task<Result<UserId, IError>> TokenRequire(HttpRequest request);
}

public class AuthResolver(
    ITokenService tokenService,
    IBmcUser bmcUser) : IAuthResolver
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly IBmcUser _bmcUser = bmcUser;

    public async Task<Result<UserId, IError>> TokenRequire(HttpRequest request)
        => await TryGetCookie(request)
        .Then(rawToken => Utils.TokenTryParse(rawToken))
        .Then(token => _bmcUser.FindByUsername(token.Ident)
            .Map(user => (token, user))
        .Then(authCtx => _tokenService.ValidateWebToken(authCtx.token, authCtx.user.Token_salt)
            .Map(_ => UserId.Create(authCtx.user.Id))));


    #region: -- Private methods

    private Task<Result<string, IError>> TryGetCookie(HttpRequest request)
        => !request.Cookies.TryGetValue("auth-token", out var token)
        ? Task.FromResult(Result<string, IError>.Err(new MissingAuth("Auth failed.", nameof(TokenRequire))))
        : Task.FromResult(Result<string, IError>.Ok(token));


    #endregion: -- Private methods

}

#region: -- Errors

public record MissingAuth(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "AUTH_NOT_PASSED";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.Unauthorized;
}

#endregion: -- Errors
