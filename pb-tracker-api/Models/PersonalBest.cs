namespace pb_tracker_api.Models;

#region: -- Id
public record struct PersonalBestId(string Id, string CategoryName)
{
    public static PersonalBestId Create(string id, string categoryName) => new(id, categoryName.Trim().ToUpper());

    public static implicit operator string(PersonalBestId p) => $"{p.Id}|{p.CategoryName}";
}

#endregion: -- Id


public sealed class PersonalBest
{
    public PersonalBestId Id { get; private set; }                        // -- 'cid|Snatch'
    public string PbDescription { get; private set; } = string.Empty;     // -- description pb (weight, reps, time, etc)
    public DateOnly DateOfPb { get; private set; }                        // -- date of pb

    private PersonalBest(
        PersonalBestId id,
        string description,
        DateOnly dateOfPb)
    {
        Id = id;
        PbDescription = description;
        DateOfPb = dateOfPb;
    }

    public static PersonalBest Create(
        PersonalBestId id,
        string description,
        string dateOfPb)
    {
        return new PersonalBest(
            id,
            description,
            DateOnly.Parse(dateOfPb));
    }
}


