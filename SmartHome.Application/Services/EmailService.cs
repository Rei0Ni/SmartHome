using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using SmartHome.Application.Interfaces.Email;
using SmartHome.Application.Interfaces.Settings;
using SmartHome.Enum;
using Microsoft.AspNetCore.Identity;
using SmartHome.Domain.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace SmartHome.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailService(ISettingsRepository settingsRepository, UserManager<ApplicationUser> userManager)
        {
            _settingsRepository = settingsRepository;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var settings = await _settingsRepository.GetEmailSettingsAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(settings.SmtpServer, settings.Port, settings.UseSsl);

                if (!settings.UseSsl)
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                await client.AuthenticateAsync(settings.Username, settings.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailToAdminsAsync(LogLevel logLevel, string subject, string emailContent)
        {
            var settings = await _settingsRepository.GetEmailSettingsAsync();

            var templateFileName = logLevel switch
            {
                LogLevel.Info => "InfoTemplate.html",
                LogLevel.Warning => "WarningTemplate.html",
                LogLevel.Error => "ErrorTemplate.html",
                LogLevel.Critical => "CriticalTemplate.html",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), $"Unsupported log level: {logLevel}")
            };

            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", templateFileName);
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template for log level '{logLevel}' not found.");
            }

            var htmlContent = await File.ReadAllTextAsync(templatePath);
            htmlContent = htmlContent.Replace($"{{{{MessageContent}}}}", emailContent);

            var adminEmails = await _userManager.GetUsersInRoleAsync("Admin");
            var adminEmailAddresses = adminEmails.Select(user => user.Email).ToList();

            if (!adminEmailAddresses.Any())
            {
                throw new InvalidOperationException("No admin email addresses configured.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            foreach (var adminEmail in adminEmailAddresses)
            {
                message.To.Add(new MailboxAddress("", adminEmail));
            }
            message.Subject = $"{logLevel}: {subject}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlContent
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(settings.SmtpServer, settings.Port, settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(settings.Username, settings.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
