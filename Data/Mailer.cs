using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Net.Security;

namespace Magazynek.Data.Mailer
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    }
    public sealed class MailKitEmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;
        public MailKitEmailSender()
        {
            _opt = EnvironmentConfig.GetSmtpOptions();
        }

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var secure = _opt.UseImplicitTls ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;


            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            var body = new BodyBuilder { HtmlBody = htmlBody, TextBody = StripHtml(htmlBody) };
            msg.Body = body.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.CheckCertificateRevocation = !OperatingSystem.IsMacOS();



            await smtp.ConnectAsync(_opt.Host, _opt.Port, secure, ct);
            await smtp.AuthenticateAsync(_opt.User, _opt.Pass, ct);
            

            await smtp.SendAsync(msg, ct);
            await smtp.DisconnectAsync(true, ct);
        }

        private static string StripHtml(string html) => System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);

    }
    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 465;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string FromAddress { get; set; } = "";
        public string FromName { get; set; } = "";
        public bool UseImplicitTls { get; set; } = true;
    }
}