namespace Backend.Interfaces
{   

    //Interface voor het versturen van emails
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
