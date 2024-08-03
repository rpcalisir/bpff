
using BalkanPanoramaFilmFestival.Models.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BalkanPanoramaFilmFestival.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        // When this constructor is called, builder.Services.Configure<EmailSettings> in Program.cs will be executed,
        // the data will be taken from appsettings.json and stored into properties of EmailSettings.cs,
        // those properties filled with data will come with options object, and will be stored in _emailSettings
        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendResetPasswordMail(string resetPasswordMailLink, string toEmail)
        {
            var smptClient = new SmtpClient();

            smptClient.Host = _emailSettings.Host!;
            smptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smptClient.UseDefaultCredentials = false;
            smptClient.Port = 587;
            smptClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smptClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email!);
            mailMessage.To.Add(toEmail);

            mailMessage.Subject = "Localhost | Şifre sıfırlama linki";
            mailMessage.Body = @$"<h4>Şifrenizi yenilemek için aşağıdaki linke tıklayınız</h4>
                            <p><a href='{resetPasswordMailLink}'>şifre yenileme link</p></a>";
            mailMessage.IsBodyHtml = true;

            await smptClient.SendMailAsync(mailMessage);
        }
    }
}
