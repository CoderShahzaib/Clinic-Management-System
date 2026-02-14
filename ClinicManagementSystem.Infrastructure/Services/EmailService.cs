using ClinicManagementSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var smtp = new SmtpClient
        {
            Host = _config["EmailSettings:SmtpServer"],
            Port = int.Parse(_config["EmailSettings:Port"]),
            EnableSsl = true,
            Credentials = new NetworkCredential(
                _config["EmailSettings:Username"],
                _config["EmailSettings:Password"]
            )
        };

        var message = new MailMessage(
            _config["EmailSettings:From"],
            to,
            subject,
            body
        )
        {
            IsBodyHtml = true
        };

        await smtp.SendMailAsync(message);
    }
}
