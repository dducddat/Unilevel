using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class EmailServices : IEmailServices
    {
        public readonly IConfiguration _configuration;

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(EmailDTO email)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = email.Body
            };

            SmtpClient client = new SmtpClient();
            client.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            client.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
            
        } 
    }
}
