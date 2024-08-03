namespace BalkanPanoramaFimlFestival.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordMail(string resetPasswordMailLink, string toEmail);
    }
}
