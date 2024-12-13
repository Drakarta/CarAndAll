using System;
using System.Net;
using System.Net.Mail;

namespace Backend.ConceptFiles
{
    public class email_test
    {
        public void TestEmail()
        {
            try
            {
                // Gmail credentials
                string fromEmail = "carandall38@gmail.com"; // Replace with your Gmail address
                string appPassword = "jgqv wpbf mjxl ddyc";  // Replace with your app password

                // Recipient email
                string toEmail = "pbt05@hotmail.nl"; // Replace with recipient's email

                // Email content
                string subject = "Test Email from C#";
                string body = "This is a test email sent using Gmail SMTP and C#.";

                // Configure SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, appPassword),
                    EnableSsl = true
                };

                // Create the MailMessage object
                MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body);

                // Send the email
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