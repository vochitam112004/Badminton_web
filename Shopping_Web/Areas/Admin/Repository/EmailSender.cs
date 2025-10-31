using Microsoft.Extensions.Options;
using Shopping_Web.Models;
using System.Net;
using System.Net.Mail;
namespace Shopping_Web.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSetting _emailSetting;
        public EmailSender(IOptions<EmailSetting> emailSettingOptions)
        {
            _emailSetting = emailSettingOptions.Value;
        }
        public Task SendEmailAsync(string email, string subject , string message)
        {
            var client = new SmtpClient(_emailSetting.SmtpServer, _emailSetting.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSetting.SenderEmail , _emailSetting.Password)
            };
            return client.SendMailAsync(
                new MailMessage(from : _emailSetting.SenderEmail,   
                                to : email ,
                                subject ,
                                message)
                {
                });
        }
    }
}
