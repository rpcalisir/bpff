namespace BalkanPanoramaFilmFestival.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordMail(string resetPasswordMailLink, string toEmail);
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
