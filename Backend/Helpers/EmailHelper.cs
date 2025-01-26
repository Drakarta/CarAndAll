using System;
using System.Reflection.Metadata.Ecma335;


namespace Backend.Helpers
{
    public static class EmailHelper
    {

        public static bool CheckAmountAllowedToAddToCompany(int abonnementMaxNumbers, int emails)
        {
            var valid = (abonnementMaxNumbers != 0 && abonnementMaxNumbers >= emails) ? true : false;
            return valid;

        }

        public static bool CheckDomeinAllowToAddToCompany(string email, string domein)
        {
            string[] emailSplit = email.Split('@');
            return emailSplit[1] == domein;
        }
    }
}
