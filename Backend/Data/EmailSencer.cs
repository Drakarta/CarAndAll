using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Backend.Data
{
    public class EmailSencer
    {
        private readonly IConfiguration _configuration;

        public EmailSencer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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