using System;


namespace Backend.Helpers
{
    public static class EmailHelper
    {
       
        public static bool CheckAmountAllowedToAddToCompany(int abonnementMaxNumbers, int emails)
        {
            if (abonnementMaxNumbers >= emails)
            {
                return true;
            } else {
                return false;
            }
        }

        public static bool CheckDomeinAllowToAddToCompany(string email, string domein)
        {
            string[] emailSplit = email.Split('@');
            return emailSplit[1] == domein;
        }
    }
}
