using System.Net;
using Azure;
using Azure.Data.Tables;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Infrastructure;

namespace pb_tracker_api.Repositories;


#region: -- Models
public class PbEntity : ITableEntity
{
#pragma warning disable CS8618 
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Pbdesc { get; set; }
    public string SWEDateOfPb { get; set; }
#pragma warning restore CS8618
}

#endregion: -- Models


// TODO: Should not be upsert but add
public interface IPbRepo
{
    Task<Result<PbEntity, IError>> UpsertPb(PbEntity pb);
}

public class PbRepo(ITableClientFactory factory) : IPbRepo
{
    private readonly Task<Result<TableClient, IError>> _tableClient = factory.GetTableClientByKey(TableClientTable.Pb);

    public async Task<Result<PbEntity, IError>> UpsertPb(PbEntity pb)
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
                return Task.FromResult(Result<PbEntity, IError>.Err(new PbQueryError($"{ex}", nameof(UpsertPb))));
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
