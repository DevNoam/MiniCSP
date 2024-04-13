namespace _365.Core.Properties
{
    public class Token
    {
        public required string otp { get; set; }
        /// <summary>
        /// Remaining seconds for this current OTP
        /// </summary>
        public int remSeconds { get; set; }
    }
}
