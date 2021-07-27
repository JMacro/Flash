using System.Collections.Generic;

namespace Flash.Extensions.Tracting.Skywalking
{
    public interface ICarrierHeaderCollection : IEnumerable<KeyValuePair<string, string>>
    {
        void Add(string key, string value);
    }
}
