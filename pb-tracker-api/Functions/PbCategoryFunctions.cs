using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Auth;
using pb_tracker_api.Bmc;
using pb_tracker_api.Extensions;

namespace pb_tracker_api.FunctionsM;

public class PbCategoryFunctions(
    IBmcPbCategory bmc,
    IAuthResolver authResolver)
{
    private readonly IBmcPbCategory _bmc = bmc;
    private readonly IAuthResolver _authResolver = authResolver;

    [Function("AddCategory")]
    public async Task<IActionResult> AddCategory([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbCategoryCreateReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.AddCategory(request.command with { Cid = request.id }))
        .MapIActionResultErrOrOk();


    [Function("GetCategories")]
    public async Task<IActionResult> GetCategories([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
      => await _authResolver.TokenRequire(req)
        .Then(id => _bmc.GetCategories(id))
        .MapIActionResultErrOrOk();


    [Function("DeleteCategory")]
    public async Task<IActionResult> DeleteCategory([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
      => await req.TryReadFromJsonAsync<PbCategoryDeleteReq>()
        .Then(command => _authResolver.TokenRequire(req)
            .Map(id => (command, id)))
        .Then(request => _bmc.DeleteCategory(request.command with { Cid = request.id }))
        .MapIActionResultErrOrOk();

}
