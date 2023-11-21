using BanPhimCanhCach.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BanPhimCanhCach.Services
{
        public class MailServices : IEmailSender
        {
            private readonly MailSettings mailSettings;

            private readonly ILogger<MailServices> logger;

            // mailSetting được Inject qua dịch vụ hệ thống
            // Có inject Logger để xuất log
            public MailServices(IOptions<MailSettings> _mailSettings, ILogger<MailServices> _logger)
            {
                mailSettings = _mailSettings.Value;
                logger = _logger;
           
            }

            public async Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                var message = new MimeMessage();
                message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
                message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlMessage;
                message.Body = builder.ToMessageBody();

                // dùng SmtpClient của MailKit
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                try
                {
                    smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {
                    // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                    System.IO.Directory.CreateDirectory("mailssave");
                    var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                    await message.WriteToAsync(emailsavefile);

                    logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                    logger.LogError(ex.Message);
                }

                smtp.Disconnect(true);

                logger.LogInformation("send mail to: " + email);

            }
        }
}
