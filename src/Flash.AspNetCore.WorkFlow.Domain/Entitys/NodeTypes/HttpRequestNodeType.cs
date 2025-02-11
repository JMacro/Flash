using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeTypes
{
    /// <summary>
    /// HTTP节点类型
    /// </summary>
    public class HttpRequestNodeType : NodeType
    {
        public string Method { get; set; }
        public string URL { get; set; }
        public IDictionary<string, string> Header { get; set; }
        public string Body { get; set; }
        public int Timeout { get; set; }
        public int Retry { get; set; }
        public string MediaType { get; set; }
    }
}
