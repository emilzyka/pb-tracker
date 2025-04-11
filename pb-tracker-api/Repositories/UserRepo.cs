using Azure;
using Azure.Data.Tables;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Repositories;

#region: -- Models
public class UserEntity : ITableEntity
{
#pragma warning disable CS8618 
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string Pwd { get; set; }
    public string Pwd_salt { get; set; }
    public string Token_salt { get; set; }
#pragma warning restore CS8618
}
#endregion: -- Models
public class UserRepo
{
    private readonly TableClient _tableClient = new TableClient("connectionstring", "table"); // move to factory

    public Task<Result<Option<User>, IError>> FirstByUsername(string username)
    {
        try
        {
            Pageable<UserEntity> entities = _tableClient.Query<UserEntity>(filter: $"RowKey eq '{username}'");

            if (!entities.Any())
            {
                return Task.FromResult(Result<Option<User>, IError>.Ok(Option<User>.None()));
            }

            return Task.FromResult(Result<Option<User>, IError>.Ok(Option<User>.Some(entities.First().ToUser())));
        }
        catch (RequestFailedException ex)
        {
            Task.FromResult(Result<Option<User>, IError>.Err(new UnknownError()); // TODO: Add error detailing caught error
        }
    }

}


#region -- Ext
public static class UserExt
{
    public static User ToUser(this UserEntity userEntity)
        => User.Restore(
            UserId.Create(userEntity.PartitionKey),
            userEntity.RowKey,
            userEntity.Pwd,
            userEntity.Pwd_salt,
            userEntity.Token_salt);

}
#endregion -- Ext



