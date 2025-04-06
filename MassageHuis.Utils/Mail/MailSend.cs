using MassageHuis.Util.Mail;
using MassageHuis.Util.Mail.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net.Sockets;

namespace MassageHuis.Util
{
    public class EmailSend : IEmailSend
    {
        private readonly EmailSettings _emailSettings;

        public EmailSend(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var mail = new MailMessage();  // create a mail object

            mail.To.Add(new MailAddress(email));
            mail.From = new MailAddress("hesselbrian@gmail.com");  // your Gmail address here
            mail.Subject = subject;
            mail.Body = message;

            mail.IsBodyHtml = true;

            try
            {
                using (var smtp = new SmtpClient(_emailSettings.MailServer))
                {
                    smtp.Port = _emailSettings.MailPort;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);
                    smtp.UseDefaultCredentials = false;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
            }
        }
    }
}
