using FluentValidation;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Extensions;
using pb_tracker_api.Models;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Bmc;


#region -- Models 
public record PbCreateReq(UserId Cid, string ExerciseName, string PbDesc, string DateOfPb);
public record PbDeleteReq(UserId Cid, string Id, string ExerciseName);

#endregion -- Models 

public interface IBmcPb
{
    Task<Result<PersonalBest, IError>> AddPb(PbCreateReq req);
    Task<Result<bool, IError>> DeletePb(PbDeleteReq req);
    Task<Result<List<PersonalBest>, IError>> GetAlPbsUser(UserId req);

}

// -- Backend Model Controller


public class BmcPb(
    IPbRepo pbRepo,
    IValidator<PersonalBest> pbValidator) : IBmcPb
{
    private readonly IPbRepo _pbRepo = pbRepo;
    private readonly IValidator<PersonalBest> _pbValidator = pbValidator;


    public async Task<Result<PersonalBest, IError>> AddPb(PbCreateReq req)
    {
        var pb = PersonalBest.Create(req.Cid, req.ExerciseName, req.PbDesc, req.DateOfPb);

        return await ValidationExt.ValidateOrError(pb, _pbValidator)
            .Map(validPb => new PbEntity
            {
                PartitionKey = req.Cid.Id,
                RowKey = pb.DateOfPb,
                Pbdesc = pb.PbDesc,
                ExerciseName = pb.ExerciseName
            })
        .Then(pbEntity => _pbRepo.AddPb(pbEntity));
    }

    public async Task<Result<bool, IError>> DeletePb(PbDeleteReq req)
       => await _pbRepo.DeletePb(req.Id, req.ExerciseName);

    public async Task<Result<List<PersonalBest>, IError>> GetAlPbsUser(UserId req)
        => await _pbRepo.GetAllPbsByUser(req);

    #region: -- Private methods

    #endregion: -- Private methods
}
