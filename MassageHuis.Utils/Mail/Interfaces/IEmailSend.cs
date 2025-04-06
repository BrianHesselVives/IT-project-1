using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassageHuis.Util.Mail.Interfaces
{
    public interface IEmailSend
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
