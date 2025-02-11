using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes
{
    /// <summary>
    /// 条件节点类型
    /// </summary>
    public class ConditionNodeType : NodeType
    {
        public string Expression { get; set; }
        public INodeType TrueNode
        {
            get
            {
                return SubNodes[0];
            }
            set
            {
                SubNodes[0] = value;
            }
        }
        public INodeType FalseNode
        {
            get
            {
                return SubNodes[1];
            }
            set
            {
                SubNodes[1] = value;
            }
        }
    }
}
