using System.Net;
using Azure;
using Azure.Data.Tables;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Infrastructure;
using pb_tracker_api.Models;

namespace pb_tracker_api.Repositories;


#region: -- Models
public class PbEntity : ITableEntity
{
#pragma warning disable CS8618 
    public string PartitionKey { get; set; }    // 'cid|category_name'
    public string RowKey { get; set; }          // uuid
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string PbDescription { get; set; }
    public string DateOfPb { get; set; }

#pragma warning restore CS8618
}

#endregion: -- Models


public interface IPbRepo
{
    Task<Result<PbEntity, IError>> AddPb(PbEntity pb);
    Task<Result<bool, IError>> DeletePb(string partitionKey, string rowKey);
    Task<Result<List<PbEntity>, IError>> GetPbsByUserAndCat(PersonalBestId id);
}

public class PbRepo(ITableClientFactory factory) : IPbRepo
{
    private readonly Task<Result<TableClient, IError>> _tableClient = factory.GetTableClientByKey(TableClientTable.Pb);

    public async Task<Result<PbEntity, IError>> AddPb(PbEntity pb)
        => await _tableClient
        .Then(client =>
        {
            try
            {
                var result = client.UpsertEntity(pb);
                return Task.FromResult(Result<PbEntity, IError>.Ok(pb));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<PbEntity, IError>.Err(new PbQueryError($"{ex}", nameof(AddPb))));
            }
        });

    public async Task<Result<bool, IError>> DeletePb(string partitionKey, string rowKey)
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
                    return Task.FromResult(Result<bool, IError>.Err(new PbQueryError($"{ex}", nameof(DeletePb))));
                }
            });

    public async Task<Result<List<PbEntity>, IError>> GetPbsByUserAndCat(PersonalBestId id)
     => await _tableClient.Then(async client =>
     {
         try
         {
             var query = client.QueryAsync<PbEntity>(e => e.PartitionKey == id);

             var result = new List<PbEntity>();

             await foreach (var entity in query)
             {
                 result.Add(entity);
             }

             return Result<List<PbEntity>, IError>.Ok(result);
         }
         catch (Exception ex)
         {
             return Result<List<PbEntity>, IError>.Err(new PbQueryError(ex.ToString(), nameof(GetPbsByUserAndCat)));
         }
     });

}

#region: -- Errors 
public record PbQueryError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "PB_QUERY_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}
#endregion: -- Errors 

