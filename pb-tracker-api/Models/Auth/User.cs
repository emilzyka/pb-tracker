namespace pb_tracker_api.Models.Auth;

#region: -- Id
public record struct UserId(string Id)
{
    public static UserId Create(string id) => new(id);

    public static UserId Void() => new(string.Empty);

    public static implicit operator string(UserId userId) => userId.Id.ToString();
}

#endregion: -- Id

public class User
{
    public UserId Id { get; private set; }          // -- PartitionKey
    public string Username { get; private set; }    // -- RowKey
    public string Pwd { get; private set; }
    public string Pwd_salt { get; private set; }
    public string Token_salt { get; private set; }

    private User(
        string username,
        string pwd)
    {
        Id = UserId.Create(Guid.NewGuid().ToString());
        Username = username;
        Pwd = pwd;
        Pwd_salt = Guid.NewGuid().ToString();
        Token_salt = Guid.NewGuid().ToString();
    }

    private User(
        UserId id,
        string username,
        string pwd,
        string pwd_salt,
        string token_salt)
    {
        Id = id;
        Username = username;
        Pwd = pwd;
        Pwd_salt = pwd_salt;
        Token_salt = token_salt;
    }

    public static User Create(
        string username,
        string pwd)
    {
        return new User(username, pwd);
    }

    public static User Restore(
        UserId id,
        string username,
        string pwd,
        string pwd_salt,
        string token_salt)
    {
        return new User(id, username, pwd, pwd_salt, token_salt);
    }
}

