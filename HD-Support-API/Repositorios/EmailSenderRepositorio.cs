using HD_Support_API.Repositorios.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HD_Support_API.Repositorios
{
    public class EmailSenderRepositorio : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string mensagem)
        {
            var mail = "hdsuport@hotmail.com";
            var senha = "hdsupport@senha";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, senha)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = mensagem,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
