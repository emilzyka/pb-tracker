using System.Net;

namespace pb_tracker_api.Abstractions;

public interface IError
{
    string ErrorMessage { get; }
    HttpStatusCode StatusCode { get; }
    string ErrorCode { get; }
    string ErrorSourceMethod { get; }
    DateTime Timestamp { get; }

    static readonly IError None = new UnknownError();
}

#region: -- General errors
public record ConfigMissingError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "CONFIG_MISSING_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}

public record DeserializationError(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "DESERIALIZE_FAILED_ERROR";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}


public record InvalidFormat(string ErrorMessage, string ErrorSourceMethod) : IError
{
    public string ErrorCode => "INVALID_FORMAT";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}

public record UnknownError : IError
{
    public string ErrorMessage => "Unknown error occurred.";
    public string ErrorCode => "UNKNOWN_ERROR";
    public string ErrorSourceMethod => "Unkown source";
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    HttpStatusCode IError.StatusCode => HttpStatusCode.InternalServerError;
}
#endregion: -- General errors

