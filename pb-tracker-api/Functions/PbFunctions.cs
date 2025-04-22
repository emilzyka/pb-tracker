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

    [Function("AddPb")]
    public async Task<IActionResult> AddPb([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbCreateReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.AddPb(request.command with { Cid = request.id }))
        .MapIActionResultErrOrOk();


    [Function("GetAllPbUser")]
    public async Task<IActionResult> GetAllPbUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbCatGetReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.GetAllPbsUserAnCat(request.command with { Cid = request.id }))
        .MapIActionResultErrOrOk();


    [Function("RemovePb")]
    public async Task<IActionResult> RemovePb([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbDeleteReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.DeletePb(request.command with { Cid = request.id }))
        .MapIActionResultErrOrOk();

}
