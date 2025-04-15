using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Auth;
using pb_tracker_api.Bmc;
using pb_tracker_api.Extensions;

namespace pb_tracker_api.Functions;

public class PbFunctions(
    IBmcPb bmc,
    IAuthResolver authResolver)
{
    private readonly IBmcPb _bmc = bmc;
    private readonly IAuthResolver _authResolver = authResolver;

    [Function("UpsertPb")]
    public async Task<IActionResult> Register([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbCreateReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.UpsertPb(request.command with { cid = request.id }))
        .MapIActionResultErrOrOk();

}
