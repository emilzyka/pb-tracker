using System.Net;
using Azure;
using Azure.Data.Tables;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Infrastructure;
using pb_tracker_api.Models;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Repositories;


#region: -- Models
public class PbEntity : ITableEntity
{
#pragma warning disable CS8618 
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string ExerciseName { get; set; }
    public string Pbdesc { get; set; }
#pragma warning restore CS8618
}

#endregion: -- Models


public interface IPbRepo
{
    Task<Result<PersonalBest, IError>> AddPb(PbEntity pb);
    Task<Result<bool, IError>> DeletePb(string partitionKey, string rowKey);
    Task<Result<List<PersonalBest>, IError>> GetAllPbsByUser(UserId userId);
}

public class PbRepo(ITableClientFactory factory) : IPbRepo
{
    private readonly Task<Result<TableClient, IError>> _tableClient = factory.GetTableClientByKey(TableClientTable.Pb);

    public async Task<Result<PersonalBest, IError>> AddPb(PbEntity pb)
        => await _tableClient
        .Then(client =>
        {
            try
            {
                var result = client.UpsertEntity(pb);
                return Task.FromResult(Result<PersonalBest, IError>.Ok(pb.ToPersonalBest()));
            }
            catch (Exception ex)
            {
                return Task.FromResult(Result<PersonalBest, IError>.Err(new PbQueryError($"{ex}", nameof(AddPb))));
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

    public async Task<Result<List<PersonalBest>, IError>> GetAllPbsByUser(UserId userId)
    => await _tableClient.Then(async client =>
    {
        try
        {
            var query = client.QueryAsync<PbEntity>(e => e.PartitionKey == userId.Id);

            var result = new List<PersonalBest>();

            await foreach (var entity in query)
            {
                result.Add(entity.ToPersonalBest());
            }

            return Result<List<PersonalBest>, IError>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<List<PersonalBest>, IError>.Err(new PbQueryError(ex.ToString(), nameof(GetAllPbsByUser)));
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

