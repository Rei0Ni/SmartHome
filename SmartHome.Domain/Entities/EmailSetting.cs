using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class EmailSetting
    {
        public Guid Id { get; set; }
        public string SmtpServer { get; private set; }
        public int Port { get; private set; }
        public string SenderEmail { get; private set; }
        public string SenderName { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool UseSsl { get; private set; } 

        public EmailSetting(string smtpServer, int port, string senderEmail, string senderName, string username, string password, bool useSsl)
        {
            SmtpServer = smtpServer;
            Port = port;
            SenderEmail = senderEmail;
            SenderName = senderName;
            Username = username;
            Password = password;
            UseSsl = useSsl;
        }

        public void Update(string smtpServer, int port, string senderEmail, string senderName, string username, string password, bool useSsl)
        {
            SmtpServer = smtpServer;
            Port = port;
            SenderEmail = senderEmail;
            SenderName = senderName;
            Username = username;
            Password = password;
            UseSsl = useSsl;
        }
    }
}
