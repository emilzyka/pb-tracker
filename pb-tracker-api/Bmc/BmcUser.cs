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

}

public class BmcUser(
    IUserRepo _userRepo,
    IPwdService pwdService)
{
    private readonly IUserRepo userRepo = _userRepo;
    private readonly IPwdService _pwdService = pwdService;

    public async Task<Result<UserId, IError>> Login(UserLogin user)
        => await userRepo
               .FirstByUsername(user.Username)
               .Then(maybeUser => maybeUser.ToAsyncResult(new UserNotFoundError("Username does not exist", nameof(Login))))
               .Then(async dbUser =>
               {
                   // -- Validate password
                   var encContent = EncryptContent.Create(user.Pwd, dbUser.Pwd_salt);
                   var pwdValid = await _pwdService.ValidatePwd(encContent, dbUser.Pwd);
                   return pwdValid.Match(
                       _ => Result<UserId, IError>.Ok(UserId.Create(dbUser.Id)),
                       err => Result<UserId, IError>.Err(err)
                   );
               });


    public async Task<Result<UserId, IError>> Register(UserRegister user)
        => await userRepo
            .FirstByUsername(user.Username)
            .Then(maybeUser => CheckIfUserExists(maybeUser))
            .Then(_ => CreateUserRecord(user))
            .Then(newUser => userRepo.Insert(newUser))
            .Map(inserted => UserId.Create(inserted.PartitionKey));



    #region -- Private Methods
    private Task<Result<UserId, IError>> CheckIfUserExists(Option<User> maybeUser)
        => maybeUser.IsNone
            ? Task.FromResult(Result<UserId, IError>.Ok(UserId.Void()))
            : Task.FromResult(Result<UserId, IError>.Err(new UserAlreadyExists("Username already exists", nameof(CheckIfUserExists))));


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

public record UserNotFoundError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "USER_NOT_FOUND";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.NotFound;
}

#endregion: -- Errors
