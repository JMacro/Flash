namespace Flash.Extensions.Office
{
    /// <summary>
    /// Sheet属性设置
    /// </summary>
    public class SheetSetting
    {
        /// <summary>
        /// Sheet属性设置
        /// </summary>
        public SheetSetting()
        {
        }
        /// <summary>
        /// 是否添加序号字段（自增序）
        /// </summary>
        public bool IsAutoNumber { get; set; } = false;
        /// <summary>
        /// 序号字段名称，默认值：序号
        /// </summary>
        public string AutoNumberName { get; set; } = "序号";
        /// <summary>
        /// 数据行高，默认值16
        /// </summary>
        public short DataRowHeight { get; set; } = 16;
        /// <summary>
        /// 标题头行高，默认值16
        /// </summary>
        public short HeaderRowHeight { get; set; } = 16;
        /// <summary>
        /// 在查看时(显示/隐藏)网格线，默认值显示
        /// </summary>
        public bool DisplayGridlines { get; set; } = true;
        /// <summary>
        /// 在打印时(显示/隐藏)网格线，默认值不显示
        /// </summary>
        public bool IsPrintGridlines { get; set; } = false;
    }
}
