using FluentValidation;
using pb_tracker_api.Abstractions;

namespace pb_tracker_api.Extensions;

public static class ValidationExt
{
    public static async Task<Result<T, IError>> ValidateOrError<T>(
        this T request,
        IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (validationResult.IsValid)
        {
            return Result<T, IError>.Ok(request);
        }
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return Result<T, IError>.Err(new ValidationError(string.Join(", ", errors), nameof(ValidateOrError)));
    }

}
