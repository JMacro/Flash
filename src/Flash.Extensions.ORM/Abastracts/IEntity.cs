using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flash.Extensions.ORM
{
    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity<TKeyType> :
        IEntity2CreateTime,
        IEntity2CreateUserId<TKeyType>,
        IEntity2ModifyTime,
        IEntity2ModifyUserId<TKeyType>,
        IEntity
        where TKeyType : struct
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [Description("主键Id")]
        TKeyType Id { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [Description("是否删除")]
        bool IsDelete { get; set; }
    }

    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity2CreateTime : IEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity2CreateUserId<TKeyType> : IEntity where TKeyType : struct
    {
        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        TKeyType CreateUserId { get; set; }
    }

    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity2ModifyTime : IEntity
    {
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Description("最后修改时间")]
        DateTime LastModifyTime { get; set; }
    }

    /// <summary>
    /// Db实体接口
    /// </summary>
    public interface IEntity2ModifyUserId<TKeyType> : IEntity where TKeyType : struct
    {
        /// <summary>
        /// 最后修改用户
        /// </summary>
        [Description("最后修改用户")]
        TKeyType LastModifyUserId { get; set; }
    }
}
