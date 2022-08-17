namespace Flash.Extensions.Job
{
    public interface IGlobalJobConfiguration
    {
        /// <summary>
        /// 仪表盘路径
        /// </summary>
        string DashboardPath { get; set; }
        /// <summary>
        /// 配置节点名称
        /// </summary>
        string SectionName { get; set; }
    }
}
