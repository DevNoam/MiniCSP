using _365.Core.Properties;
using OtpNet;

public class TokenDecrypt
{
    public static Token GetNumbers(string token)
    {
        byte[] secret;
        try
        {
            secret = Base32Encoding.ToBytes(token.Trim());
        }
        catch (Exception)
        {
            return new Token() {otp = "ERROR", remSeconds = 0 };
        }

        // Generate TOTP code
        var totp = new Totp(secret);
        var code = totp.ComputeTotp(DateTime.UtcNow);
        int time = totp.RemainingSeconds();
        Token returnToken = new Token {
            otp = code,
            remSeconds = time
        };

        return returnToken;
    }
}
