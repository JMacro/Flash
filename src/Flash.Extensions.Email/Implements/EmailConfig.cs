namespace Flash.Extensions.Email
{
    public class EmailConfig
    {
        /// <summary>
        /// 发件人名称
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public string FromEmail { get; set; }
        /// <summary>
        /// SMTP服务器
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// 是否开启Ssl
        /// </summary>
        public bool SmtpEnableSsl { get; set; } = true;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPwd { get; set; }
    }
}
