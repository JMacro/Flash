using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.Email
{
    public class EmailService : IEmailService
    {
        private readonly IMailKitProvider _mailKitProvider;

        public EmailService(IMailKitProvider mailKitProvider)
        {
            this._mailKitProvider = mailKitProvider;
        }

        #region Send
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, mailCcList, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, mailCcList, null, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, null, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, null, null, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfo">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, null, null, subject, content, encoding, isHtml, sender, new List<AttachmentInfo> { attachmentInfo });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, mailBccList, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, null, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfo">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, new List<AttachmentInfo> { attachmentInfo });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, mailCcList, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, mailCcList, null, subject, content, encoding, isHtml, sender, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, null, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public void Send(List<string> mailToList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            SendEmail(mailToList, null, null, subject, content, encoding, isHtml, sender, null);
        }
        #endregion

        #region SendAsync
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, mailCcList, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, mailCcList, null, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, null, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, null, null, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, mailBccList, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, null, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="mailCc">抄送人</param>
        /// <param name="mailBcc">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfo">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(new List<string> { mailTo }, new List<string> { mailCc }, new List<string> { mailBcc }, subject, content, encoding, isHtml, sender, new List<AttachmentInfo> { attachmentInfo });
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, mailCcList, mailBccList, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, mailCcList, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, mailCcList, null, subject, content, encoding, isHtml, sender, null);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, null, null, subject, content, encoding, isHtml, sender, attachmentInfoList);
           });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <returns></returns>
        public Task SendAsync(List<string> mailToList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null)
        {
            return Task.Factory.StartNew(() =>
           {
               SendEmail(mailToList, null, null, subject, content, encoding, isHtml, sender, null);
           });
        }
        #endregion

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailToList">收件人</param>
        /// <param name="mailCcList">抄送人</param>
        /// <param name="mailBccList">密送人</param>
        /// <param name="subject">标题</param>
        /// <param name="content">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="isHtml">是否HTML内容</param>
        /// <param name="sender">发件人信息</param>
        /// <param name="attachmentInfoList">附件</param>
        /// <returns></returns>
        private void SendEmail(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null, List<AttachmentInfo> attachmentInfoList = default)
        {
            Check.Argument.IsNotEmpty(mailToList, nameof(mailToList));
            Check.Argument.IsNotEmpty(content, nameof(content));

            var mimeMessage = new MimeMessage();

            //add mail from
            if (!string.IsNullOrEmpty(sender?.FromEmail) && !string.IsNullOrEmpty(sender?.FromName))
            {
                mimeMessage.From.Add(new MailboxAddress(sender.FromName, sender.FromEmail));
            }
            else
            {
                mimeMessage.From.Add(new MailboxAddress(this._mailKitProvider.Options.FromName, this._mailKitProvider.Options.FromEmail));
            }

            //add mail to
            foreach (var to in mailToList)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(to));
            }

            //add mail cc
            if (mailCcList != null && mailCcList.Any())
            {
                foreach (var cc in mailCcList)
                {
                    mimeMessage.Cc.Add(MailboxAddress.Parse(cc));
                }
            }

            //add mail bcc
            if (mailBccList != null && mailBccList.Any())
            {
                foreach (var bcc in mailBccList)
                {
                    mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc));
                }
            }

            mimeMessage.Subject = subject;

            //add email body
            TextPart body = null;

            if (isHtml)
            {
                body = new TextPart(TextFormat.Html);
            }
            else
            {
                body = new TextPart(TextFormat.Text);
            }

            //set email encoding
            body.SetText(encoding, content);

            //add multipart
            Multipart multipartBody = new Multipart("mixed")
            {
                body
            };

            // add attachments
            if (attachmentInfoList != null && attachmentInfoList.Any())
            {
                foreach (var attach in attachmentInfoList)
                {
                    if (string.IsNullOrEmpty(attach.MainMimeType) || string.IsNullOrEmpty(attach.SubMimeType))
                    {
                        multipartBody.Add(new MimePart()
                        {
                            IsAttachment = true,
                            Content = new MimeContent(attach.FileStream, ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = attach.FileName,
                        });
                    }
                    else
                    {
                        multipartBody.Add(new MimePart(attach.MainMimeType, attach.SubMimeType)
                        {
                            IsAttachment = true,
                            Content = new MimeContent(attach.FileStream, ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = attach.FileName,
                        });
                    }
                }
            }

            //set email body
            mimeMessage.Body = multipartBody;

            using (var client = this._mailKitProvider.SmtpClient)
            {
                client.Send(mimeMessage);
            }
        }
    }
}
