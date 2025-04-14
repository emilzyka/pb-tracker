using System.Text.Json;
using Microsoft.AspNetCore.Http;
using pb_tracker_api.Abstractions;

namespace pb_tracker_api.Extensions;

public static class HttpExt
{
    public async static Task<Result<T, IError>> TryReadFromJsonAsync<T>(this HttpRequest request)
    {
        try
        {
            T? result = await request.ReadFromJsonAsync<T>();

            return result != null
                ? Result<T, IError>.Ok(result)
                : Result<T, IError>.Err(new DeserializationError("Deserialize was null", nameof(TryReadFromJsonAsync)));
        }
        catch (JsonException ex)
        {
            return Result<T, IError>.Err(new DeserializationError(ex.Message, nameof(TryReadFromJsonAsync)));
        }
    }
}
