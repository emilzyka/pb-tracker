using Azure;
using Azure.Data.Tables;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Infrastructure;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Repositories;


#region: -- Models
public class PbCategoryEntity : ITableEntity
{
#pragma warning disable CS8618 
    public string PartitionKey { get; set; }    // cid (creator id)
    public string RowKey { get; set; }          // category_name
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
#pragma warning restore CS8618
}

#endregion: -- Models


public interface IPbCategoryRepo
{
    Task<Result<PbCategoryEntity, IError>> AddPbCategory(PbCategoryEntity pb);
    Task<Result<bool, IError>> DeletePbCategory(string partitionKey, string rowKey);
    Task<Result<List<PbCategoryEntity>, IError>> GetAllCategoriesByUser(UserId userId);
}

public class PbCategoryRepo(ITableClientFactory factory) : IPbCategoryRepo
{
    private readonly Task<Result<TableClient, IError>> _tableClient = factory.GetTableClientByKey(TableClientTable.PbCategory);

    public async Task<Result<PbCategoryEntity, IError>> AddPbCategory(PbCategoryEntity pb)
        => await _tableClient
        .Then(client =>
        {
            try
            {
                var result = client.UpsertEntity(pb);
                return Task.FromResult(Result<PbCategoryEntity, IError>.Ok(pb));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<PbCategoryEntity, IError>.Err(new PbQueryError($"{ex}", nameof(AddPbCategory))));
            }
        });

    public async Task<Result<bool, IError>> DeletePbCategory(string partitionKey, string rowKey)
        => await _tableClient
            .Then(client =>
            {
                try
                {
                    client.DeleteEntity(partitionKey, rowKey);
                    return Task.FromResult(Result<bool, IError>.Ok(true));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(Result<bool, IError>.Err(new PbQueryError($"{ex}", nameof(DeletePbCategory))));
                }
            });

    public async Task<Result<List<PbCategoryEntity>, IError>> GetAllCategoriesByUser(UserId userId)
        => await _tableClient.Then(async client =>
        {
            try
            {
                var query = client.QueryAsync<PbCategoryEntity>(e => e.PartitionKey == userId.Id);

                var result = new List<PbCategoryEntity>();

                await foreach (var entity in query)
                {
                    result.Add(entity);
                }

                return Result<List<PbCategoryEntity>, IError>.Ok(result);
            }
            catch (Exception ex)
            {
                return Result<List<PbCategoryEntity>, IError>.Err(new PbQueryError(ex.ToString(), nameof(GetAllCategoriesByUser)));
            }
        });

}
