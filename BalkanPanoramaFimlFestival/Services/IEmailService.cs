namespace BalkanPanoramaFilmFestival.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordMail(string resetPasswordMailLink, string toEmail);
    }
}
