using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Auth;

#region: -- Errors
public record PwdNotMatching(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "PWD_NOT_MATCHING";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.Forbidden;
}
#endregion: -- Errors


#region: -- Models

// Could use unit type instead
public enum PwdValidationRes
{
    Success
}
#endregion: -- Models

public interface IPwdService
{
    Task<Result<PwdValidationRes, IError>> ValidatePwd(EncryptContent encContent, string pwdRef);
    Task<Result<string, IError>> EncryptPwd(EncryptContent encContent);
}

public class PwdService(IConfiguration config) : IPwdService
{
    private readonly IConfiguration _config = config;

    public Task<Result<PwdValidationRes, IError>> ValidatePwd(EncryptContent encContent, string pwdRef)
        => EncryptPwd(encContent)
            .Then(encPwd =>
            {
                if (encPwd == pwdRef)
                {
                    return Task.FromResult(Result<PwdValidationRes, IError>.Ok(PwdValidationRes.Success));
                }

                return Task.FromResult(Result<PwdValidationRes, IError>.Err(new PwdNotMatching("Password not matching", nameof(ValidatePwd))));
            });


    public Task<Result<string, IError>> EncryptPwd(EncryptContent encContent)
        => _config["SERVICE_PWD_KEY"]
            .ToOption()
            .MapNoneErrAsync(new ConfigMissingError("pwd key not found", nameof(EncryptPwd)))
            .Then(key =>
            {
                var encRes = EncryptIntoB64U(Convert.FromBase64String(key), encContent);
                return Task.FromResult(Result<string, IError>.Ok($"#01#{encRes}"));
            });


    public string EncryptIntoB64U(byte[] key, EncryptContent encContent)
    {
        using var hmac = new HMACSHA512(key);

        var contentBytes = Encoding.UTF8.GetBytes(encContent.Content);
        var saltBytes = Encoding.UTF8.GetBytes(encContent.Salt);

        hmac.TransformBlock(contentBytes, 0, contentBytes.Length, null, 0);
        hmac.TransformFinalBlock(saltBytes, 0, saltBytes.Length);

        var hash = hmac.Hash!;
        return Base64UrlEncode(hash);
    }

    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
