using System.Net.Mail;

namespace MassageHuis.Util.Mail.Interfaces
{
    public interface IEmailSend
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendReservationEmailAsync(string email, string subject, string message, Attachment attachment);
    }
}
