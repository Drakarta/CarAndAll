using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Backend.Data
{
    public static class Password
    {

        //hier wordt een random password gegenereerd voor een zakelijke klant/ wagenparkbeheerder die nog niet een account heeft bij CarAndAll en wordt toegevoegd via het wagenparkbeheer panel
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            var salt = BC.EnhancedHashPassword(res.ToString());
            return salt;
        }
    }
}