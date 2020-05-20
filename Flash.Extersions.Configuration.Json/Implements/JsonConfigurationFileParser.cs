using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Flash.Extersions.Configuration.Json
{
    internal class JsonConfigurationFileParser
    {
        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;
        private JsonTextReader _reader;

        private JsonConfigurationFileParser()
        {
        }

        public static IDictionary<string, string> Parse(Stream input)
        {
            return new JsonConfigurationFileParser().ParseStream(input);
        }

        private IDictionary<string, string> ParseStream(Stream input)
        {
            _data.Clear();
            _reader = new JsonTextReader(new StreamReader(input));
            _reader.DateParseHandling = DateParseHandling.None;
            JObject jObject = JObject.Load(_reader);
            VisitJObject(jObject);
            return _data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (JProperty item in jObject.Properties())
            {
                EnterContext(item.Name);
                VisitProperty(item);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;

                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;

                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token);
                    break;

                default:
                    throw new FormatException(string.Format(
                       "UnsupportedJSONToken",
                        _reader.TokenType,
                        _reader.Path,
                        _reader.LineNumber,
                        _reader.LinePosition));
            }
        }

        private void VisitArray(JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                EnterContext(i.ToString());
                VisitToken(array[i]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JToken data)
        {
            string currentPath = _currentPath;
            if (_data.ContainsKey(currentPath))
            {
                throw new ArgumentException(string.Format("ArgumentIsNullOrWhitespace", currentPath));
            }
            //处理配置中环境变量参数
            _data[currentPath] = EnvironmentHelper.GetEnvironmentVariable(data.ToString());
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
