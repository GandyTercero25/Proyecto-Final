using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ECommerceArtesanos.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("soulwhite95@gmail.com", "wrvtgcnlwdbfhddg");
                client.UseDefaultCredentials = false;


                client.Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(
                        _config["EmailSettings:From"],
                        _config["EmailSettings:DisplayName"]);

                    message.To.Add(email);
                    message.Subject = subject;
                    message.Body = htmlMessage;
                    message.IsBodyHtml = true;

                    try
                    {
                        await client.SendMailAsync(message);
                        Console.WriteLine("Correo enviado correctamente a " + email);
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine("Error SMTP: " + smtpEx.Message);
                        throw; // Puedes manejarlo o relanzar
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error general enviando correo: " + ex.Message);
                        throw;
                    }
                }
            }
        }
    }
}
