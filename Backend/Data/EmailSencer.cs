using System;
using System.Net;
using System.Net.Mail;
using Backend.Interfaces;

namespace Backend.Data
{
    public class EmailSencer : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSencer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //Hier wordt de email verstuurd via de gmail van CarAndAll
        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string fromEmail = _configuration["GMAIL:Email"];
                string appPassword = _configuration["GMAIL:Password"];
                string smtpServer = _configuration["GMAIL:SmtpServer"];
                int smtpPort = int.Parse(_configuration["GMAIL:SmtpPort"]);

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, appPassword),
                    EnableSsl = true
                };

                MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body);

                smtpClient.Send(mailMessage);

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}


//copy dit in de controller waar je de email wilt versturen
//  private readonly EmailSencer _emailSencer; boven aan de controller
//  _emailSencer = emailSencer; in de constructor
// dit waar je de email wilt versturen:
// string context = $"{model.Email} {password} Dit zijn uw loggin gegevens";
// _emailSencer.SendEmail("voeg het email dat verstuurd wordt hier", "Account gegevens", context);