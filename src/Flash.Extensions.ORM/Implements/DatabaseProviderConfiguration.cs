namespace Flash.Extensions.ORM
{
    /// <summary>
    /// 数据库提供商配置
    /// </summary>
    public class DatabaseProviderConfiguration
    {
        /// <summary>
        /// 数据库提供商类型
        /// </summary>
        public DatabaseProviderType ProviderType { get; set; } = DatabaseProviderType.MySql;
    }

    /// <summary>
    /// 数据库提供商类型
    /// </summary>
    public enum DatabaseProviderType
    {
        /// <summary>
        /// SqlServer
        /// </summary>
        SqlServer,
        /// <summary>
        /// PostgreSQL
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// MySql
        /// </summary>
        MySql
    }
}
