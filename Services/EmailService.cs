using System.Net;
using System.Net.Mail;

namespace SportComplex.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public void SendEmail(string to, string subject, string body)
    {
        try
        {
            string smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            int smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
            string smtpUser = _config["Email:User"] ?? "";
            string smtpPass = _config["Email:Password"] ?? "";

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUser, smtpPass)
            };

            var message = new MailMessage(smtpUser, to, subject, body);
            client.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email error: {ex.Message}");
        }
    }
}