namespace pb_tracker_api.Abstractions;

public readonly struct Option<T>
{
    private readonly T? _value;
    private bool HasValue { get; }

    private Option(T? value)
    {
        HasValue = true;
        _value = value!;
    }

    #region -- Access methods
    public Option() => HasValue = false;
    public bool IsSome => HasValue;
    public bool IsNone => !HasValue;

    /// <summary>
    /// Gets the value contained in the <see cref="Option{T}"/> if it exists.
    /// Unsafe should only be used after checking <see cref="IsSome"/>.
    /// </summary>
    public T Value => _value!;
    #endregion -- Access methods


    #region -- Static factory methos 
    public static Option<T> None() => new Option<T>();
    public static Option<T> Some(T value) => new Option<T>(value);

    #endregion -- Static factory methos 

    /// <summary>
    /// Retrieves the value contained in the <see cref="Option{T}"/> if it exists.
    /// Else throws an expection with the provided error message or default message.
    /// In production use only when the absence of a value is considered a unrecoverable error. 
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns>
    /// The value contained in the <see cref="Option{T}"/> if it exists.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <see cref="Option{T}"/> has no value (i.e., <see cref="HasValue"/> is false).
    /// </exception>
    public T Expect(string? errorMessage = null)
        => HasValue ? _value! : throw new InvalidOperationException(errorMessage ?? "Option has no value");

    /// <summary>
    /// This function can be used to pass through a some option while handling the none option with an IError. 
    /// successful result while handling an error.
    /// </summary>
    /// <returns>
    /// </returns>
    public Result<T, IError> MapNoneErr(IError error)
        => HasValue ? Result<T, IError>.Ok(_value!) : Result<T, IError>.Err(error);

    public Task<Result<T, IError>> MapNoneErrAsync(IError error)
        => HasValue ? Task.FromResult(Result<T, IError>.Ok(_value!)) : Task.FromResult(Result<T, IError>.Err(error));

    public T GetValueOrDefault(T defaultValue)
        => HasValue ? _value! : defaultValue;

}

#region -- OptionExt

public static class OptionExt
{
    public static Option<T> ToOption<T>(this T? nullableValue)
        where T : struct
        => nullableValue.HasValue ? Option<T>.Some(nullableValue.Value) : Option<T>.None();

    public static Option<T> ToOption<T>(this T? value)
        where T : class
        => value != null ? Option<T>.Some(value) : Option<T>.None();

    public static async Task<Option<T>> ToOptionAsync<T>(this Task<T> task)
    {
        var value = await task;
        return value is not null ? Option<T>.Some(value) : Option<T>.None();
    }

    public static Option<T> FirstOrOption<T>(this IEnumerable<T>? source)
        => source == null || !source.Any() ? Option<T>.None() : Option<T>.Some(source.First());

    public static TResult Match<T, TResult>(this Option<T> option, Func<T, TResult> some, Func<TResult> none)
        => option.IsSome ? some(option.Value) : none();

    public static Task<Result<T, IError>> ToAsyncResult<T>(this Option<T> option, IError error)
        => option.IsSome ? Task.FromResult(Result<T, IError>.Ok(option.Value)) : Task.FromResult(Result<T, IError>.Err(error));

}


#endregion -- OptionExt


