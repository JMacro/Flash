
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flash.Test.Web
{
    /// <summary>
    /// 账户信息
    /// </summary>
    [Table("AccountInfo")]
    public partial class AccountInfo
    {
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(32)")]
        public string Account { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(32)")]
        public string CName { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(32)")]
        public string EName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(32)")]
        public string CellphoneNumber { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(64)")]
        public string Email { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(32)")]
        public string Password { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        [Required]
        [DefaultValue("")]
        public string ImagePath { get; set; }
    }
}
