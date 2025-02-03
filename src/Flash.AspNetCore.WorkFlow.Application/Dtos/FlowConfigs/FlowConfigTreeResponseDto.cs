using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Application.Dtos.FlowConfigs
{
    public class FlowConfigTreeResponseDto : FlowConfigResponseDto
    {
        /// <summary>
        /// 是否显示 
        /// </summary>
        public bool Display { get; set; } = true;
        /// <summary>
        /// 子级
        /// </summary>
        public List<FlowConfigTreeResponseDto> Children { get; set; } = new List<FlowConfigTreeResponseDto>();
    }
}
