
using Flash.Extensions.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flash.Test
{
    [TestClass]
    public class EmailTest : BaseTest
    {
        [TestMethod]
        public void TestSendEmail()
        {
            var tool = ServiceProvider.GetService<IEmailService>();
            Assert.IsNotNull(tool);

            tool.Send("XXXX@163.com", "邮箱发送测试", "邮箱发送测试", System.Text.Encoding.UTF8);
        }
    }
}
