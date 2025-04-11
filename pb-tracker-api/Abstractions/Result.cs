using Microsoft.AspNetCore.Mvc;

namespace pb_tracker_api.Abstractions;

public class Result<T, E>
{
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(IError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IError? Error { get; }
    public T? Value { get; }

    public static Result<T, E> Ok(T value) => new(value);
    public static Result<T, E> Err(IError error) => new(error);

    public TRes Match<TRes>(Func<T, TRes> onOk, Func<IError, TRes> onErr)
        => IsSuccess ? onOk(Value!) : onErr(Error!);

}


#region: -- Result Ext

public static class ResultExt
{
    /// <summary>
    /// Chains async computations. If the current Result is Ok, the provided async function is executed.
    /// If it's an error, the error is propagated as-is.
    /// </summary>
    public static async Task<Result<TResult, TError>> Then<T, TResult, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<Result<TResult, TError>>> bindFunc)
    {
        Result<T, TError> result = await resultTask;

        return await result.Match(
            ok => bindFunc(ok),
            err => Task.FromResult(Result<TResult, TError>.Err(err))
        );
    }

    /// <summary>
    /// Maps a successful async Result value to another type,
    /// while propagating any existing error.
    /// </summary>
    public static async Task<Result<TResult, TError>> Map<T, TResult, TError>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TResult> mapFunc)
    {
        Result<T, TError> result = await resultTask;

        return result.Match(
            ok => Result<TResult, TError>.Ok(mapFunc(ok)),
            err => Result<TResult, TError>.Err(err)
        );
    }

    public static IActionResult MapIActionResultErrOrOk<T, IError>(
        this Result<T, IError> result)
    {
        return result.Match<IActionResult>(
            ok => new OkObjectResult(ok),
            err => new ObjectResult(err) { StatusCode = (int)err.StatusCode }
       );
    }
}


#endregion: -- Result Ext
