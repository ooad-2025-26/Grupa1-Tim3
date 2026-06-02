using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EVrtic.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        // Jednostavna regex provjera za format email adrese
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string toEmail, string subject, string body)
        {
            // Skip ako email nije ispravnog formata
            if (string.IsNullOrWhiteSpace(toEmail) || !EmailRegex.IsMatch(toEmail))
            {
                _logger.LogWarning("Preskačem slanje — neispravan format email adrese: {Email}", toEmail);
                return false;
            }

            try
            {
                var smtpServer   = _config["EmailSettings:SmtpServer"]   ?? "smtp.gmail.com";
                var smtpPort     = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
                var senderEmail  = _config["EmailSettings:SenderEmail"]  ?? "";
                var senderName   = _config["EmailSettings:SenderName"]   ?? "eVrtić";
                var senderPwd    = _config["EmailSettings:SenderPassword"] ?? "";

                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPwd))
                {
                    _logger.LogError("SMTP nije konfigurisan (SenderEmail ili SenderPassword nedostaje u appsettings.json)");
                    return false;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, senderPwd),
                    Timeout = 10000
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email uspješno poslan na {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slanje emaila na {Email} nije uspjelo: {Poruka}", toEmail, ex.Message);
                return false;
            }
        }
    }
}
