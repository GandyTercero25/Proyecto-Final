using System.Net.Mail;
using System.Net;

namespace ECommerceArtesanos.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
