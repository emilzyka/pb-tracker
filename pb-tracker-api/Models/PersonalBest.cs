using pb_tracker_api.Models.Auth;

namespace pb_tracker_api.Models;

public class PersonalBest
{
    public UserId Cid { get; set; }                             // -- created by id
    public string ExerciseName { get; set; } = string.Empty;    // -- exercise name
    public string Pbdesc { get; set; } = string.Empty;          // -- description pb (weight, reps, time, etc)
    public string SWEDateOfPb { get; set; } = string.Empty;     // -- date of pb (swe)
}
