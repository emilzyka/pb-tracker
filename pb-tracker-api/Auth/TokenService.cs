using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using pb_tracker_api.Abstractions;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Auth;

public interface ITokenService
{
    Task<Result<Token, IError>> GenerateWebToken(string user, string salt);
    Task<Result<string, IError>> ValidateWebToken(Token token, string salt);
}

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;

    public Task<Result<Token, IError>> GenerateWebToken(string user, string salt)
        => GenerateToken(
            _config["SERVICE_TOKEN_KEY"].ToOption(),
            _config["SERVICE_TOKEN_DURATION_SEC"].ToOption(),
            user,
            salt);

    public Task<Result<string, IError>> ValidateWebToken(Token token, string salt)
        => ValidateTokenSignAndExp(
            _config["SERVICE_TOKEN_KEY"].ToOption(),
            token,
            salt);

    private async Task<Result<Token, IError>> GenerateToken(Option<string> key, Option<string> durationSec, string username, string salt)
        => await key.ToAsyncResult(new ConfigMissingError("Token key not found", nameof(GenerateToken)))
            .Then(key => durationSec.ToAsyncResult(new ConfigMissingError("Duration not found in", nameof(GenerateToken)))
            .Map(dur => (key, dur))
            .Then(kd =>
            {
                string exp = DateTime.UtcNow.AddSeconds(int.Parse(kd.dur)).ToString("o");
                string sign = TokenSignIntoB64U(username, exp, salt, Utils.Base64UrlDecode(kd.key));

                return Task.FromResult(Result<Token, IError>.Ok(Token.Create(username, exp, sign)));
            }));


    private async Task<Result<string, IError>> ValidateTokenSignAndExp(Option<string> key, Token token, string salt)
        => await key.ToAsyncResult(new ConfigMissingError("Token key not found", nameof(ValidateTokenSignAndExp)))
            .Then(key =>
            {
                string expectedSign = TokenSignIntoB64U(token.Ident, token.Exp, salt, Utils.Base64UrlDecode(key));

                if (!DateTime.TryParse(token.Exp, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expTime))
                {
                    return Task.FromResult(Result<string, IError>.Err(new InvalidFormat("Token invalid format.", nameof(ValidateTokenSignAndExp))));
                }

                if (DateTime.UtcNow > expTime)
                {
                    return Task.FromResult(Result<string, IError>.Err(new TokenExpired("Token expired.", nameof(ValidateTokenSignAndExp))));
                }

                return Task.FromResult(Result<string, IError>.Ok(string.Empty));
            });


    #region: -- Private metods
    private string TokenSignIntoB64U(string ident, string exp, string salt, byte[] key)
    {
        var encodedIdent = Utils.Base64UrlEncode(Encoding.UTF8.GetBytes(ident));
        var encodedExp = Utils.Base64UrlEncode(Encoding.UTF8.GetBytes(exp));
        var content = $"{encodedIdent}.{encodedExp}";
        return EncryptIntoB64U(key, content, salt);
    }

    private string EncryptIntoB64U(byte[] key, string content, string salt)
    {
        using var hmac = new HMACSHA256(key);
        var combined = Encoding.UTF8.GetBytes(content + salt);
        var hash = hmac.ComputeHash(combined);
        return Utils.Base64UrlEncode(hash);
    }

    #endregion: -- Private metods


}

#region: -- Errors
public record TokenNotMatching(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "TOKEN_NOTMATCHING_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}

public record TokenExpired(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "TOKEN_NOTMATCHING_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}
#endregion: -- Errors

