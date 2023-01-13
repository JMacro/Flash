using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;

namespace Flash.Extensions.Email
{
    public class MailKitProvider : IMailKitProvider
    {
        public EmailConfig Options { get; private set; }

        public MailKitProvider(EmailConfig mailKitOptions)
        {
            Options = mailKitOptions;
        }

        #region Smtp

        public SmtpClient SmtpClient
        {
            get
            {
                return lazySmtpClient().Value;
            }
        }
        private Lazy<SmtpClient> lazySmtpClient()
        {
            return new Lazy<SmtpClient>(() =>
            {
                return InitSmtpClient();
            });
        }

        private SmtpClient InitSmtpClient()
        {
            var client = new SmtpClient();

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;


            if (!Options.SmtpEnableSsl)
            {
                client.Connect(Options.SmtpServer, Options.SmtpPort, SecureSocketOptions.None);
            }
            else
            {
                // fix issue #6
                client.Connect(Options.SmtpServer, Options.SmtpPort, SecureSocketOptions.Auto);
            }

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // user login smtp server (fix issue #9)
            if (!string.IsNullOrEmpty(Options.UserName) && !string.IsNullOrEmpty(Options.UserPwd))
            {
                client.Authenticate(Options.UserName, Options.UserPwd);
            }

            return client;
        }

        #endregion

        #region Pop3
        public Pop3Client Pop3Client
        {
            get
            {
                return lazyPop3Client().Value;
            }
        }
        private Lazy<Pop3Client> lazyPop3Client()
        {
            return new Lazy<Pop3Client>(() =>
            {
                return InitPop3Client();
            });
        }

        private Pop3Client InitPop3Client()
        {
            var client = new Pop3Client();

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(Options.SmtpServer, Options.SmtpPort, Options.SmtpEnableSsl);

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");

            // user login pop3 server
            client.Authenticate(Options.UserName, Options.UserPwd);
            return client;
        }

        #endregion

        #region Imap
        public ImapClient ImapClient
        {
            get
            {
                return lazyImapClient().Value;
            }
        }
        private Lazy<ImapClient> lazyImapClient()
        {
            return new Lazy<ImapClient>(() =>
            {
                return InitImapClient();
            });
        }

        private ImapClient InitImapClient()
        {
            var client = new ImapClient();

            client.Connect(Options.SmtpServer, Options.SmtpPort, Options.SmtpEnableSsl);

            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            // user login imap server
            client.Authenticate(Options.UserName, Options.UserPwd);

            return client;
        }
        #endregion
    }
}
