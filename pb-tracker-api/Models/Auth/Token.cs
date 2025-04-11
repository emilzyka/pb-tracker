namespace pb_tracker_api.Models.Auth;

public class Token
{
    public string Ident { get; set; }       // Username
    public string Exp { get; set; }         // Expiration in ISO 8601
    public string SignB64U { get; set; }    // Signature base64url

    private Token(
        string ident,
        string exp,
        string signB64U)
    {
        Ident = ident;
        Exp = exp;
        SignB64U = signB64U;
    }

    public static Token Create(
        string ident,
        string exp,
        string signB64U
        )
    {
        return new Token(
            ident,
            exp,
            signB64U);
    }

}
