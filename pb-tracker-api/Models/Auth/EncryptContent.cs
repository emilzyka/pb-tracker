namespace pb_tracker_api.Models.Auth;

public class EncryptContent
{
    public string Content { get; set; }
    public string Salt { get; set; }

    private EncryptContent(
        string content,
        string salt)
    {
        Content = content;
        Salt = salt;
    }

    public static EncryptContent Create(
        string content,
        string salt)
    {
        return new EncryptContent(
            content,
            salt);
    }
}
