using FluentValidation;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Bmc;


#region -- Models 
public record PbCreateReq(UserId Cid, string CategoryName, string PbDescription, string DateOfPb);
public record PbCatGetReq(UserId Cid, string CategoryName);
public record PbDeleteReq(UserId Cid, string CategoryName, string Rowkey);

#endregion -- Models 


#region: -- Validation
public class PersonalBestValidator : AbstractValidator<PersonalBest>
{
    public PersonalBestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.PbDescription)
            .NotEmpty()
            .WithMessage("Description name is required.")
            .MaximumLength(200).WithMessage("Descriptionmust be less than 200 characters.");
    }
}
#endregion: -- Validation

public interface IBmcPb
{
    Task<Result<PbEntity, IError>> AddPb(PbCreateReq req);
    Task<Result<bool, IError>> DeletePb(PbDeleteReq req);
    Task<Result<List<PbEntity>, IError>> GetAllPbsUserAnCat(PbCatGetReq req);
}

// -- Backend Model Controller

public class BmcPb(
    IPbRepo repo) : IBmcPb
{
    private readonly IPbRepo _repo = repo;
    //private readonly IValidator<PersonalBest> _Validator = pbValidator;


    public async Task<Result<PbEntity, IError>> AddPb(PbCreateReq req)
    {
        var id = PersonalBestId.Create(
            req.Cid,
            req.CategoryName);

        var pb = PersonalBest.Create(
            id,
            req.PbDescription,
            req.DateOfPb);

        var entity = new PbEntity
        {
            PartitionKey = pb.Id,
            RowKey = Guid.NewGuid().ToString(),
            PbDescription = pb.PbDescription,
            DateOfPb = pb.DateOfPb.ToString()
        };

        return await _repo.AddPb(entity);
    }

    public async Task<Result<bool, IError>> DeletePb(PbDeleteReq req)
       => await _repo.DeletePb(PersonalBestId.Create(req.Cid.Id, req.CategoryName), req.Rowkey);

    public async Task<Result<List<PbEntity>, IError>> GetAllPbsUserAnCat(PbCatGetReq req)
        => await _repo.GetPbsByUserAndCat(PersonalBestId.Create(req.Cid.Id, req.CategoryName));

}
