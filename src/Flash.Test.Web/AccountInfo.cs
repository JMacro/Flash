
using System;
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
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifiedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 创建人名称
        /// </summary>
        [DefaultValue("")]
        [Column(TypeName = "varchar(16)")]
        public virtual string CreateName { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        [Column(TypeName = "varchar(16)")]
        public virtual string OperatorName { get; set; }
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
        /// 学生性别
        /// </summary>
        [Required]
        public int Gender { get; set; }
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
        /// 密码盐
        /// </summary>
        [Required]
        [DefaultValue("")]
        [Column(TypeName = "varchar(32)")]
        public string PasswordSalt { get; set; } = "";
        /// <summary>
        /// 头像地址
        /// </summary>
        [Required]
        [DefaultValue("")]
        public string ImagePath { get; set; }
        /// <summary>
        /// 账户状态
        /// </summary>
        [Required]
        public int Status { get; set; }
    }
}
