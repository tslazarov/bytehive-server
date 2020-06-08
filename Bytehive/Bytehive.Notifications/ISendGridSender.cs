using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Notifications
{
    public interface ISendGridSender
    {
        Task<HttpStatusCode> SendMessage(string fromEmail, string fromName, string toEmail, string subject, string plainTextContent, string htmlContent);

        string GetResetPasswordPlainText(string randomNumber, string language);

        string GetResetPasswordHtml(string randomNumber, string language);

        string GetRequestReadyPlainText(string language);

        string GetRequestReadyHtml(string language);
    }
}
