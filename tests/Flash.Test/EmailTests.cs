
using Autofac;
using Flash.Extensions.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Flash.Test
{
    [TestFixture]
    public class EmailTests
    {
        private EmailConfig option;
        private MailKitProvider mailKitProvider;
        private EmailService tool;

        public EmailTests()
        {
            option = new EmailConfig();
            mailKitProvider = new MailKitProvider(option);
            tool = new EmailService(mailKitProvider);
        }

        [Test]
        public void SendAsError4FromNameAndFromEmailTest()
        {
            option = new EmailConfig();
            mailKitProvider = new MailKitProvider(option);
            tool = new EmailService(mailKitProvider);

            Assert.That(new TestDelegate(() =>
            {
                tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
            }), new ThrowsExceptionConstraint());
        }

        [Test]
        public void SendAsError4SmtpServerAndSmtpPortTest()
        {
            option = new EmailConfig();
            option.FromName = "example";
            option.FromEmail = "user@163.com";
            mailKitProvider = new MailKitProvider(option);
            tool = new EmailService(mailKitProvider);

            Assert.That(new TestDelegate(() =>
            {
                tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
            }), new ThrowsExceptionConstraint());
        }

        [Test]
        public void SendAsError4AuthenticationTest()
        {
            option = new EmailConfig();
            option.FromName = "example";
            option.FromEmail = "user@163.com";
            option.SmtpServer = "smtp.163.com";
            option.SmtpPort = 465;
            option.SmtpEnableSsl = true;
            mailKitProvider = new MailKitProvider(option);
            tool = new EmailService(mailKitProvider);

            Assert.That(new TestDelegate(() =>
            {
                tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
            }), new ThrowsExceptionConstraint());
        }

        [Test]
        public void SendAsError4UserNameAndUserPwdTest()
        {
            option = new EmailConfig();
            option.FromName = "example";
            option.FromEmail = "user@163.com";
            option.SmtpServer = "smtp.163.com";
            option.SmtpPort = 465;
            option.SmtpEnableSsl = true;
            option.UserName = "UserName";
            option.UserPwd = "UserPwd";
            mailKitProvider = new MailKitProvider(option);
            tool = new EmailService(mailKitProvider);

            Assert.That(new TestDelegate(() =>
            {
                tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
            }), new ThrowsExceptionConstraint());
        }
    }
}
