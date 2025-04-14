using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Auth;
using pb_tracker_api.Bmc;
using pb_tracker_api.Extensions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Functions;

public class UserFunctions(
    IBmcUser bmc,
    ITokenService tokenService)
{
    private readonly IBmcUser _bmc = bmc;
    private readonly ITokenService _tokenService = tokenService;

    [Function("Register")]
    public async Task<IActionResult> Register([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        => await req
            .TryReadFromJsonAsync<UserRegister>()
            .Then(request => _bmc.Register(request))
            .MapIActionResultErrOrOk();

    [Function("Login")]
    public async Task<IActionResult> Login([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
     => await req
         .TryReadFromJsonAsync<UserLogin>()
         .Then(request => _bmc.Login(request))
         .Then(dbUser => _tokenService.GenerateWebToken(dbUser.Username, dbUser.Token_salt)
         .Map(token => (dbUser, token)))
         .Then(ut =>
         {
             Utils.SetTokenCookie(req.HttpContext.Response, ut.token);
             return Task.FromResult(Result<UserId, IError>.Ok(ut.dbUser.Id));
         })
         .MapIActionResultErrOrOk();

}
