namespace pb_tracker_api.Bmc;


#region -- Models 

public record UserLogin(string Username, string Pwd);

#endregion -- Models 


// -- Backend Model Controller

public interface IBmcUser
{

}

public class BmcUser
{

    /*
    public Result<UserId, IError> Login(UserLogin user)
    {
        // Step 1: Get the user from repo
        var userRes = _repo.FindByUsername(user.Username);
        if (userRes.IsNone)
            return Result<UserId, IError>.Err(new NotFoundError("User not found", nameof(Login)));

        var dbUser = userRes.Value;

        // Step 2: Validate password
        var encContent = new EncryptContent
        {
            Content = user.Pwd,
            Salt = dbUser.Salt
        };

        var pwdValid = _pwdService.ValidatePwd(encContent, dbUser.HashedPwd).Result; // Or `await` if this becomes async

        return pwdValid.Match(
            _ => Result<UserId, IError>.Ok(new UserId(dbUser.Id)),
            err => Result<UserId, IError>.Err(err)
        );
    }
    */
}
