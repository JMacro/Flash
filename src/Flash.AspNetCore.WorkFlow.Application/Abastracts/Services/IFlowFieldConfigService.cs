using Flash.AspNetCore.WorkFlow.Application.Dtos.FlowFieldConfigs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flash.AspNetCore.WorkFlow.Application.Abastracts.Services
{
    /// <summary>
    /// 工作流配置(字段)
    /// </summary>
    public interface IFlowFieldConfigService
    {
        /// <summary>
        /// 保存工作流字段配置
        /// </summary>
        /// <param name="workFlowFields"></param>
        void SaveData(List<FlowFieldConfigRequestDto> workFlowFields);
        /// <summary>
        /// 获得工作流字段列表
        /// </summary>
        /// <param name="workFlowModuleSceneConfigId"></param>
        /// <returns></returns>
        Task<List<FlowFieldConfigResponseDto>> GetFields(long workFlowModuleSceneConfigId);
        /// <summary>
        /// 测试字段执行方法
        /// </summary>
        /// <param name="fieldId"></param>
        void TestFieldExecuteMethod(long fieldId);
    }
}
