using _365.Core.Properties;
using OtpNet;

public class TokenDecrypt
{
    public static Token GetNumbers(string token)
    {

        var secretBytes = Base32Encoding.ToBytes(token.Trim());

        // Generate TOTP code
        var totp = new Totp(secretBytes);
        var code = totp.ComputeTotp(DateTime.UtcNow);
        int time = totp.RemainingSeconds();
        Token returnToken = new Token {
            otp = code,
            remSeconds = time
        };

        return returnToken;
    }
}
