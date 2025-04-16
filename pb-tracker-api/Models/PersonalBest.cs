using FluentValidation;
using pb_tracker_api.Models.Auth;
using pb_tracker_api.Repositories;

namespace pb_tracker_api.Models;

// Also need some input validation

public sealed class PersonalBest
{
    public UserId Cid { get; private set; }                             // -- created by id
    public string ExerciseName { get; private set; } = string.Empty;    // -- exercise name
    public string PbDesc { get; private set; } = string.Empty;          // -- description pb (weight, reps, time, etc)
    public string DateOfPb { get; private set; } = string.Empty;        // -- date of pb (swe)

    private PersonalBest(
        UserId cid,
        string exerciseName,
        string pbdesc,
        string dateOfPb)
    {
        Cid = cid;
        ExerciseName = exerciseName;
        PbDesc = pbdesc;
        DateOfPb = dateOfPb;
    }

    public static PersonalBest Create(
        UserId cid,
        string exerciseName,
        string pbdesc,
        string timeofPb)
    {
        return new PersonalBest(
            cid,
            exerciseName.ToLower().Trim(),
            pbdesc,
            timeofPb);
    }

    public static PersonalBest Restore(
       UserId cid,
       string exerciseName,
       string pbdesc,
       string timeofPb)
    {
        return new PersonalBest(
            cid,
            exerciseName,
            pbdesc,
            timeofPb);
    }

}

#region: -- Validation
public class PersonalBestValidator : AbstractValidator<PersonalBest>
{
    public PersonalBestValidator()
    {
        RuleFor(x => x.Cid).NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.ExerciseName)
            .NotEmpty()
            .WithMessage("Exercise name is required.")
            .MaximumLength(100).WithMessage("Exercise name must be less than 50 characters.");

        RuleFor(x => x.PbDesc)
            .NotEmpty()
            .WithMessage("Pb description is required.");

        RuleFor(x => x.DateOfPb)
            .NotEmpty()
            .WithMessage("Date of PB is required.")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Date of PB must be in the format YYYY-MM-DD.")
            .MaximumLength(100)
            .WithMessage("Date of PB must be less than 100 characters");
    }
}
#endregion: -- Validation


#region -- Ext
public static class PbEntityExt
{
    public static PersonalBest ToPersonalBest(this PbEntity pbEntity)
        => PersonalBest.Restore(
            UserId.Create(pbEntity.PartitionKey),
            pbEntity.ExerciseName,
            pbEntity.Pbdesc,
            pbEntity.RowKey);

}
#endregion -- Ext
