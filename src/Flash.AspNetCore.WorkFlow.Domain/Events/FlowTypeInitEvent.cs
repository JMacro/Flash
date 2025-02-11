using Flash.AspNetCore.WorkFlow.Domain.Core;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.FlowTypes;
using Flash.AspNetCore.WorkFlow.Domain.Entitys.NodeSettings;
using Flash.AspNetCore.WorkFlow.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flash.AspNetCore.WorkFlow.Domain.Events
{
    internal class FlowTypeInitEvent : DomainEventBase
    {
        public string BussinessName { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(EnumNodeSettingJsonConverter))]
        public IEnumerable<INodeSetting> Settings { get; set; }
        [JsonConverter(typeof(EnumableNodeTypeInfoDataJsonConvert))]
        public IEnumerable<NodeTypeInfoData> Nodes { get; set; }
        public IEnumerable<Behavior> Events { get; set; }
        public IDictionary<string, string> AttachFormDatas { get; set; }
        public string FormTypeId { get; set; }
        public bool Removed { get; set; }
        public bool IsDisable { get; set; }
        public bool Completed { get; set; }
        public string Description { get; set; }
        public bool IsDraft { get; set; }
    }

    /// <summary>
    /// 自定义序列化和反序列化
    /// </summary>
    internal class EnumableNodeTypeInfoDataJsonConvert : JsonConverter<IEnumerable<NodeTypeInfoData>>
    {
        public override IEnumerable<NodeTypeInfoData> ReadJson(JsonReader reader, Type objectType, IEnumerable<NodeTypeInfoData> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dtos = serializer.Deserialize<DataTransferObject[]>(reader);
            return dtos.Select(dto =>
            {
                var type = Type.GetType(dto.TypeFullName);
                return JsonConvert.DeserializeObject(dto.Data, type) as NodeTypeInfoData;
            }).ToArray();
        }

        public override void WriteJson(JsonWriter writer, IEnumerable<NodeTypeInfoData> value, JsonSerializer serializer)
        {
            var list = value.Select(node => new DataTransferObject
            {
                Data = JsonConvert.SerializeObject(node),
                TypeFullName = node.GetType().FullName
            }).ToArray();

            serializer.Serialize(writer, list);
        }
    }

    /// <summary>
    /// 自定义序列化和反序列化
    /// </summary>
    internal class EnumNodeSettingJsonConverter : JsonConverter<IEnumerable<INodeSetting>>
    {
        public override IEnumerable<INodeSetting> ReadJson(JsonReader reader, Type objectType, IEnumerable<INodeSetting> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dtos = serializer.Deserialize<DataTransferObject[]>(reader);
            return dtos.Select(dto =>
            {
                var type = Type.GetType(dto.TypeFullName);
                return JsonConvert.DeserializeObject(dto.Data, type) as INodeSetting;
            }).ToArray();
        }

        public override void WriteJson(JsonWriter writer, IEnumerable<INodeSetting> value, JsonSerializer serializer)
        {
            var list = value.Select(node => new DataTransferObject
            {
                Data = JsonConvert.SerializeObject(node),
                TypeFullName = node.GetType().FullName
            }).ToArray();

            serializer.Serialize(writer, list);
        }
    }
}
