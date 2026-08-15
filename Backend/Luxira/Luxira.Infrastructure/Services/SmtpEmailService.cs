using Luxira.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Luxira.Infrastructure.Services;

// Sends via a transactional email provider's SMTP relay (Brevo -
// smtp-relay.brevo.com - chosen after Gmail SMTP turned out to need direct
// login access to the destination inbox, which isn't available; Brevo needs
// only a separately-owned account with one verified sender email, free tier,
// no credit card). Credentials (Email:Username = Brevo login,
// Email:Password = Brevo SMTP key) live in User Secrets, never appsettings.
// Swappable behind IEmailService if the provider ever changes.
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var section = _configuration.GetSection("Email");
        var username = section["Username"];
        var password = section["Password"];
        var fromAddress = section["FromAddress"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fromAddress))
        {
            _logger.LogWarning("Email sending is not fully configured; skipping email send to {To}.", to);
            return;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(section["FromName"] ?? "Luxira", fromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            var host = section["SmtpHost"] ?? "smtp-relay.brevo.com";
            var port = int.TryParse(section["SmtpPort"], out var configuredPort) ? configuredPort : 587;

            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);
        }
        catch (Exception ex)
        {
            // Never let an email failure fail the caller's primary flow (e.g.
            // order creation) - log and move on.
            _logger.LogError(ex, "Failed to send email to {To}.", to);
        }
    }
}
