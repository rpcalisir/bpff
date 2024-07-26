using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BalkanPanoramaFimlFestival.Models.Account
{
    public class EmailSender : IEmailSender
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailSender(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await _fluentEmail
                .To(email)
                .Subject(subject)
                .Body(message, isHtml: true)
                .SendAsync();
        }
    }
}
