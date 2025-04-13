using System.Net;
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

public interface IUserRepo
{
    Task<Result<Option<User>, IError>> FirstByUsername(string username);
    Task<Result<UserEntity, IError>> Insert(UserEntity user);
}

public class UserRepo : IUserRepo
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
            return Task.FromResult(Result<Option<User>, IError>.Err(new UserQueryError($"{ex}", nameof(FirstByUsername))));
        }
    }

    public Task<Result<UserEntity, IError>> Insert(UserEntity user)
    {
        try
        {
            var result = _tableClient.AddEntity(user);
            return Task.FromResult(Result<UserEntity, IError>.Ok(user));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<UserEntity, IError>.Err(new UserQueryError($"{ex}", nameof(Insert))));

        }
    }

}

#region: -- Errors 

public record UserQueryError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "USER_QUERY_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}

#endregion: -- Errors 

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



