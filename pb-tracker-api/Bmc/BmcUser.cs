using System.Net;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Auth;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Bmc;


#region -- Models 
public record UserLogin(string Username, string Pwd);

public record UserRegister(string Username, string Pwd);
#endregion -- Models 


// -- Backend Model Controller

public interface IBmcUser
{
    Task<Result<User, IError>> Login(UserLogin user);
    Task<Result<UserId, IError>> Register(UserRegister user);
    Task<Result<User, IError>> FindByUsername(string username);
}

public class BmcUser(
    IUserRepo userRepo,
    IPwdService pwdService) : IBmcUser
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IPwdService _pwdService = pwdService;

    public async Task<Result<User, IError>> Login(UserLogin user)
        => await _userRepo
               .FirstByUsername(user.Username)
               .Then(maybeUser => maybeUser.ToAsyncResult(new UserNotFoundError("Username does not exist", nameof(Login))))
               .Then(async dbUser =>
               {
                   // -- Validate password
                   var encContent = EncryptContent.Create(user.Pwd, dbUser.Pwd_salt);
                   var pwdValid = await _pwdService.ValidatePwd(encContent, dbUser.Pwd);

                   return pwdValid.Match(
                       _ => Result<User, IError>.Ok(dbUser),
                       err => Result<User, IError>.Err(err)
                   );
               });

    public async Task<Result<UserId, IError>> Register(UserRegister user)
        => await _userRepo
            .FirstByUsername(user.Username)
            .Then(maybeUser => CheckIfUserNotExists(maybeUser))
            .Then(_ => CreateUserRecord(user))
            .Then(newUser => _userRepo.Insert(newUser))
            .Map(inserted => UserId.Create(inserted.PartitionKey));


    public async Task<Result<User, IError>> FindByUsername(string username)
       => await _userRepo
           .FirstByUsername(username)
           .Then(maybeUser => CheckIfUserExists(maybeUser));


    #region -- Private Methods
    private Task<Result<UserId, IError>> CheckIfUserNotExists(Option<User> maybeUser)
          => maybeUser.IsNone
              ? Task.FromResult(Result<UserId, IError>.Ok(UserId.Void()))
              : Task.FromResult(Result<UserId, IError>.Err(new UserAlreadyExists("Username already exists", nameof(CheckIfUserNotExists))));

    private Task<Result<User, IError>> CheckIfUserExists(Option<User> maybeUser)
      => maybeUser.IsSome
          ? Task.FromResult(Result<User, IError>.Ok(maybeUser.Value))
          : Task.FromResult(Result<User, IError>.Err(new UserNotFound("Username already exists", nameof(CheckIfUserExists))));

    private Task<Result<UserEntity, IError>> CreateUserRecord(UserRegister user)
        => _pwdService
            .GenerateSalt()
            .Then(salt => _pwdService.EncryptPwd(EncryptContent.Create(user.Pwd, salt))
            .Map(encPwd => (salt, encPwd))
            .Then((content) =>
            {
                var newUser = new UserEntity
                {
                    PartitionKey = UserId.Create(Guid.NewGuid().ToString()),
                    RowKey = user.Username,
                    Pwd = content.encPwd,
                    Pwd_salt = content.salt,
                    Token_salt = Guid.NewGuid().ToString()
                };
                return Task.FromResult(Result<UserEntity, IError>.Ok(newUser));
            }));

    #endregion -- Private Methods
}

#region: -- Errors

public record UserAlreadyExists(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "USERNAME_ALDREADY_EXISTS";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.NotFound;
}

public record UserNotFound(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "USER_DOES_NOT_EXISTS";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.NotFound;
}

public record UserNotFoundError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "USER_NOT_FOUND";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.NotFound;
}

#endregion: -- Errors
