using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Models;

// TODO: Time should not be generated in the backend
// Also need some input validation

public class PersonalBest
{
    public UserId Cid { get; set; }                             // -- created by id
    public string ExerciseName { get; set; } = string.Empty;    // -- exercise name
    public string PbDesc { get; set; } = string.Empty;          // -- description pb (weight, reps, time, etc)
    public string SWEDateOfPb { get; set; } = string.Empty;     // -- date of pb (swe)

    private PersonalBest(
        UserId cid,
        string exerciseName,
        string pbdesc,
        string swedateOfPb)
    {
        Cid = cid;
        ExerciseName = exerciseName;
        PbDesc = pbdesc;
        SWEDateOfPb = swedateOfPb;
    }

    public static PersonalBest Create(
        UserId cid,
        string exerciseName,
        string pbdesc)
    {
        var sweTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo
            .FindSystemTimeZoneById("W. Europe Standard Time"))
            .ToString("yyyy-MM-ddTHH:mm:ss");

        return new PersonalBest(cid, exerciseName.ToLower().Trim(), pbdesc, sweTime);
    }

}
