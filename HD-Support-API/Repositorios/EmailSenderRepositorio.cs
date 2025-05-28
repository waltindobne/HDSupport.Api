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
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("O endereço de email não pode ser vazio", nameof(email));
            }

            var mail = "walter.222brito@gmail.com";
            var senha = "ucys jzqn auyk geem";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, senha)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail, "HD Support"),
                Subject = subject,
                Body = mensagem,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
