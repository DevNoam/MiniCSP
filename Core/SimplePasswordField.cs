using Microsoft.VisualBasic;

namespace _365.Core
{
    public class SimplePasswordField
    {
        /// <summary>
        /// Made for pre-existed databases, for export/iMPORT
        /// </summary>
        public static string RequestPassword() => Interaction.InputBox("Enter database encryption password:", "Password Prompt", "", -1, -1);
        
        /// <summary>
        /// Made for newely created databses
        /// </summary>
        public static string NewDatabasePassword() => Interaction.InputBox("Enter database encryption password (optional):", "Password Prompt", "", -1, -1);
    }
}
