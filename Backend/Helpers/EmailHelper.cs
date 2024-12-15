using System;

namespace Backend.Helpers
{
    public static class EmailHelper
    {
        public static bool CheckAmountAllowedToAddToCompany(string abbonement, int emails)
        {
            if (abbonement == "kleinste" && emails >= 2)
            {
                return false;
            }
            else if (abbonement == "middel" && emails >= 5)
            {
                return false;
            }
            else if (abbonement == "groot" && emails >= 10)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckDomeinAllowToAddToCompany(string email, string domein)
        {
            string[] emailSplit = email.Split('@');
            return emailSplit[1] == domein;
        }
    }
}
