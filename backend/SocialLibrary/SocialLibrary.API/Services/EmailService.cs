using System.Net;
using System.Net.Mail;

namespace SocialLibrary.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task Gonder(string aliciEmail, string konu, string icerik)
        {
            var gonderenMail = _config["EmailAyarlari:GonderenMail"];
            var sifre = _config["EmailAyarlari:Sifre"];
            var host = _config["EmailAyarlari:Host"];
            var port = int.Parse(_config["EmailAyarlari:Port"]);

            var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(gonderenMail, sifre),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(gonderenMail, "Sosyal Kütüphane"),
                Subject = konu,
                Body = icerik,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(aliciEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}