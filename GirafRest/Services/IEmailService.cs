using System.Threading.Tasks;

namespace GirafRest.Services
{
    /// <summary>
    /// Defines an interface for an email sender, that may be used to send emails to users who have forgotten
    /// their password.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email to the given email with the given subject and message.
        /// </summary>
        /// <param name="email">The email to send to.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The message of the email.</param>
        /// <returns></returns>
        Task SendEmailAsync(string email, string subject, string message);
    }
}
