using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Commands.FlowForms.GetFlowFormDetial
{
    /// <summary>
    /// 获取表单类型详情查询结果
    /// </summary>
    internal class GetFlowFormDetialQueryResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IDictionary<string, string> FieldDatas { get; set; }
        public int Version { get; set; }
    }
}
