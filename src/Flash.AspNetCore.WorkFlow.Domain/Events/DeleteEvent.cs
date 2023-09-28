using Flash.AspNetCore.WorkFlow.Domain.Core;
using Newtonsoft.Json;
using System;

namespace Flash.AspNetCore.WorkFlow.Domain.Events
{
    internal class DeleteEvent :DomainEventBase
    {
        [JsonConverter(typeof(TypeJsonConverter))]
        public Type Type { get; set; }
        public string DeleteExcutorId { get; set; }
    }

    /// <summary>
    /// 自定义序列化和反序列化
    /// </summary>
    public class TypeJsonConverter : JsonConverter<Type>
    {
        public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var fullName = serializer.Deserialize<string>(reader);
            return Type.GetType(fullName);
        }

        public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.FullName);
        }
    }
}
