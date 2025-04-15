using pb_tracker_api.Abstractions;
using pb_tracker_api.Models;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Bmc;


#region -- Models 
public record PbCreateReq(UserId cid, string ExerciseName, string PbDesc);

#endregion -- Models 

public interface IBmcPb
{
    Task<Result<PbEntity, IError>> UpsertPb(PbCreateReq req);
}

// -- Backend Model Controller


public class BmcPb(IPbRepo pbRepo) : IBmcPb
{
    private readonly IPbRepo _pbRepo = pbRepo;

    public async Task<Result<PbEntity, IError>> UpsertPb(PbCreateReq req)
    {
        var pb = PersonalBest.Create(req.cid, req.ExerciseName, req.PbDesc);

        var pbEntity = new PbEntity
        {
            PartitionKey = req.cid.Id,
            RowKey = pb.ExerciseName,
            Pbdesc = pb.PbDesc,
            SWEDateOfPb = pb.SWEDateOfPb
        };

        return await _pbRepo.UpsertPb(pbEntity);
    }
}
