using FluentValidation;
using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Models;

public sealed class PersonalBestCategory
{
    public UserId Cid { get; private set; }
    public string CategoryName { get; private set; } = string.Empty;

    private PersonalBestCategory(
        UserId cid,
        string categoryName)
    {
        Cid = cid;
        CategoryName = categoryName;
    }

    public static PersonalBestCategory Create(
    UserId cid,
    string categoryName)
    {
        return new PersonalBestCategory(
            cid,
            categoryName.Trim().ToUpper());
    }

}

#region: -- Validation
public class PersonalBestCategoryValidator : AbstractValidator<PersonalBestCategory>
{
    public PersonalBestCategoryValidator()
    {
        RuleFor(x => x.Cid).NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("Exercise name is required.")
            .MaximumLength(100).WithMessage("Exercise name must be less than 100 characters.");

    }
}
#endregion: -- Validation
