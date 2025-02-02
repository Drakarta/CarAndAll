using System;
using System.Reflection.Metadata.Ecma335;


namespace Backend.Helpers
{
    public static class EmailHelper
    {
        //Kijkt of het emailadres mag toegeveogd worden aan het bedrijfop basis van of het aantal emails niet groter is dan het aantal emails dat mag toegevoegd worden aan het bedrijf
        public static bool CheckAmountAllowedToAddToCompany(int abonnementMaxNumbers, int emails)
        {
            var valid = (abonnementMaxNumbers != 0 && abonnementMaxNumbers >= emails) ? true : false;
            return valid;

        }
        //kijkt naar wat het domein van de email van het bedrijf is en van de gebruiker die toegevoegd wilt worden. Zijn ze niet gelijk mag hij niet toegevoegd worden.
        public static bool CheckDomeinAllowToAddToCompany(string email, string domein)
        {
            string[] emailSplit = email.Split('@');
            return emailSplit[1] == domein;
        }
    }
}
