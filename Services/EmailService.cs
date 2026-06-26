using System;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MimeKit;
using Project_StudentERP.Documents.EmailTemplates;
using Project_StudentERP.Interfaces;

namespace Project_StudentERP.Services
{
    public class EmailService : IEmailService
    {
        public readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendReceipt(
            string toEmail,
            string toParentEmail,
            string studentName,
            string receiptNo,
            decimal amountPaid,
            DateTime paymentDate,
            byte[] pdf
        )
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress("Student ERP", _emailSettings.Email));

                message.To.Add(MailboxAddress.Parse(toEmail));
                if (!string.IsNullOrWhiteSpace(toParentEmail))
                {
                    message.Cc.Add(MailboxAddress.Parse(toParentEmail));
                }

                message.Subject = "Fee Payment Receipt";

                //var path = Path.Combine(
                //    AppContext.BaseDirectory,
                //    "Documents",
                //    "EmailTemplates",
                //    "PaymentReceipt.html"
                //);
                //Console.WriteLine(path);
                //Console.WriteLine(File.Exists(path));

                string html = File.ReadAllText(
                    Path.Combine(
                        AppContext.BaseDirectory,
                        "Documents",
                        "EmailTemplates",
                        "PaymentReceipt.html"
                    )
                );

                var builder = new BodyBuilder();

                html = html.Replace("{{StudentName}}", studentName);
                html = html.Replace("{{ReceiptNo}}", receiptNo);
                html = html.Replace("{{AmountPaid}}", amountPaid.ToString("N2"));
                html = html.Replace("{{PaymentDate}}", paymentDate.ToString("dd-MMM-yyyy"));

                builder.HtmlBody = html;

                builder.Attachments.Add(
                    $"Receipt_{receiptNo}.pdf",
                    pdf,
                    new ContentType("application", "pdf")
                );

                message.Body = builder.ToMessageBody();

                // Creating smtp client connection
                using var smtp = new SmtpClient();

                Console.WriteLine("Connected");
                await smtp.ConnectAsync(
                    _emailSettings.Host,
                    _emailSettings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                Console.WriteLine("Authenticated");

                await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);

                Console.WriteLine("Sending mail");
                await smtp.SendAsync(message);

                Console.WriteLine("Mail sent");
                await smtp.DisconnectAsync(true);

                Console.WriteLine("Disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
