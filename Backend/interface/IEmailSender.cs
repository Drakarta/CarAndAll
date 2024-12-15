namespace Backend.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
