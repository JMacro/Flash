using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Extensions.Email
{
    public interface IEmailService
    {
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
        void Send(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(string mailTo, string mailCc, string mailBcc, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        void Send(List<string> mailToList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);
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
        Task SendAsync(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string content, AttachmentInfo attachmentInfo, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, List<string> mailCcList, List<string> mailBccList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, List<string> mailCcList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, List<string> mailCcList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, string subject, string content, List<AttachmentInfo> attachmentInfoList, Encoding encoding, bool isHtml = false, SenderInfo sender = null);

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
        Task SendAsync(List<string> mailToList, string subject, string content, Encoding encoding, bool isHtml = false, SenderInfo sender = null);
        #endregion
    }
}
