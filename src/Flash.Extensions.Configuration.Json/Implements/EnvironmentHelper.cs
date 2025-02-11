using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Flash.Extensions.Configuration.Json
{
    public class EnvironmentHelper
    {
        public static string GetEnvironmentVariable(string value)
        {
            var result = value;
            var paramList = GetParameters(result);
            foreach (var param in paramList)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    var env = Environment.GetEnvironmentVariable(param);
                    if (env == null)
                    {
                        throw new ArgumentNullException($"Not set environment variable,plase set Environment.SetEnvironmentVariable(\"{ param }\",\"\")");
                    }
                    result = result.Replace("${" + param + "}", env);
                }
            }
            return result;
        }

        private static List<string> GetParameters(string text)
        {
            var matchVale = new List<string>();
            string Reg = @"(?<=\${)[^\${}]*?(?=})";
            foreach (Match m in Regex.Matches(text, Reg))
            {
                matchVale.Add(m.Value);
            }
            return matchVale;
        }
    }
}
