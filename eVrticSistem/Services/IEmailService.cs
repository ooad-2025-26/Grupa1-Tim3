using System.Threading.Tasks;

namespace EVrtic.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Šalje email koristeći SMTP konfiguraciju iz appsettings.json.
        /// Vraća true ako je email uspješno predan SMTP serveru, false ako je došlo do greške.
        /// </summary>
        Task<bool> SendAsync(string toEmail, string subject, string body);
    }
}
