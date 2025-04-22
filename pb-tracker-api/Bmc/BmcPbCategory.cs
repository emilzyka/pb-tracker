using FluentValidation;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Extensions;
using pb_tracker_api.Models;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Bmc;


#region -- Models 

public record PbCategoryCreateReq(UserId Cid, string CategoryName);
public record PbCategoryDeleteReq(UserId Cid, string CategoryName);


#endregion -- Models 

public interface IBmcPbCategory
{
    Task<Result<PbCategoryEntity, IError>> AddCategory(PbCategoryCreateReq req);
    Task<Result<bool, IError>> DeleteCategory(PbCategoryDeleteReq req);
    Task<Result<List<PbCategoryEntity>, IError>> GetCategories(UserId req);

}

// -- Backend Model Controller

public class BmcPbCategory(
    IPbCategoryRepo repo,
    IValidator<PersonalBestCategory> validator) : IBmcPbCategory
{
    private readonly IPbCategoryRepo _repo = repo;
    private readonly IValidator<PersonalBestCategory> _validator = validator;


    public async Task<Result<PbCategoryEntity, IError>> AddCategory(PbCategoryCreateReq req)
    {
        var pb = PersonalBestCategory.Create(
            req.Cid,
            req.CategoryName);

        return await ValidationExt.ValidateOrError(pb, _validator)
            .Map(validPb => new PbCategoryEntity
            {
                PartitionKey = req.Cid.Id,
                RowKey = pb.CategoryName,
            })
        .Then(pbEntity => _repo.AddPbCategory(pbEntity));
    }

    public async Task<Result<bool, IError>> DeleteCategory(PbCategoryDeleteReq req)
    {
        var cat = PersonalBestCategory.Create(
            req.Cid,
            req.CategoryName);

        return await _repo.DeletePbCategory(cat.Cid, cat.CategoryName);
    }

    public async Task<Result<List<PbCategoryEntity>, IError>> GetCategories(UserId req)
        => await _repo.GetAllCategoriesByUser(req);
}
