using Flash.Core;
using Flash.Extensions.CompareObjects;

namespace Flash.Extensions.ChangeHistory
{
    public interface IEntityChangeHostBuilder : IFlashServiceCollection
    {
        /// <summary>
        /// 对比器配置
        /// </summary>
        ComparisonConfig Config { get; }
    }
}
