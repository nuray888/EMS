using Azure.Identity;
using MailKit.Net.Smtp;
using MimeKit;

namespace UnitedPayment.Services
{

    public interface IEmailService
    {
        Task SendEmailAsync(string toName,string toEmail, string subject, string text);
    }
    public class EmailService : IEmailService
    {
        private readonly string smtpServer;
        private readonly string smtpUsername;
        private readonly string smtpPassword;
        private readonly int smtpPort;
        public EmailService(IConfiguration configuration)
        {
            smtpServer = configuration.GetValue<string>("EmailSettings:SmtpServer", "");
            smtpPort = configuration.GetValue<int>("EmailSettings:Port");
            smtpUsername = configuration.GetValue<string>("EmailSettings:Username", "");
            smtpPassword = configuration.GetValue<string>("EmailSettings:Password", "");
        }

        public async Task SendEmailAsync(string toName, string toEmail, string subject, string text)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("App", smtpUsername));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;


            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, false);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);

                try
                {
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

    }
    }
