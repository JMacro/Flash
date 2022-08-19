using Flash.Extensions.Resilience.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using Flash.Extensions;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    public class HttpTestController : ControllerBase
    {
        private readonly IHttpClient _httpClient;

        public HttpTestController(IHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        [HttpGet("ResilientHttpTest")]
        public async Task<object> ResilientHttpTest()
        {
            var value = new ModelTest();
            var df = value.ToFormData();
            var login = new LoginViewModel
            {
                UserName = "JMacro",
                Password = "1",
                CompanyName = "捷扬讯科"
            };
            var dd = await this._httpClient.PostAsync("http://192.168.18.241/api/Passport/login", login, HttpMediaType.FormData);
            var content = await dd.ReadAsStringAsync();

            //return value.ToFormData();

            var dfdf = await (await this._httpClient.PostAsync("http://127.0.0.1:5555/api/HttpTest/NoExceptionTest", new { })).ReadAsStringAsync();

            try
            {
                var dfdfd = await (await this._httpClient.PostAsync("http://127.0.0.1:5555/api/HttpTest/ExceptionTest", new { })).ReadAsStringAsync();
            }
            catch (Exception)
            {

            }
            
            return new
            {
                JSON = content,
                T1 = dfdf
            };
        }

        [HttpPost("ExceptionTest")]
        public object ExceptionTest()
        {
            throw new Exception();
        }

        [HttpPost("NoExceptionTest")]
        public string NoExceptionTest()
        {
            return "yes";
        }

        private void Handle(string fieldName, object value, PropertyInfo[] properties, ref Dictionary<string, string> dic)
        {
            foreach (var propertie in properties)
            {
                var field = $"{propertie.Name}";
                if (!string.IsNullOrEmpty(fieldName))
                {
                    field = $"{fieldName}.{propertie.Name}";
                }

                if (propertie.PropertyType.IsValueType || propertie.PropertyType == typeof(string))
                {
                    dic.Add(field, propertie.GetValue(value)?.ToString() ?? "");
                }
                else if (propertie.PropertyType.IsArray)
                {
                    var elementType = propertie.PropertyType.GetElementType();
                    var isValueType = (elementType.IsValueType || elementType == typeof(string));

                    var objectValue = propertie.GetValue(value);
                    if (objectValue != null)
                    {
                        int count = Convert.ToInt32(objectValue.GetType().GetProperty("Length").GetValue(objectValue, null));
                        var getValueMethod = propertie.PropertyType.GetMethod("GetValue", new Type[] { typeof(Int32) });

                        for (int i = 0; i < count; i++)
                        {
                            object item = getValueMethod.Invoke(objectValue, new object[] { i });
                            if (isValueType)
                            {
                                dic.Add($"{field}[{i}]", item.ToString());
                            }
                            else
                            {
                                Handle($"{field}[{i}]", item, elementType.GetProperties(), ref dic);
                            }
                        }
                    }
                }
                else if (!propertie.PropertyType.IsValueType && !propertie.PropertyType.IsGenericType)
                {
                    var objectValue = propertie.GetValue(value);
                    Handle(field, objectValue, objectValue.GetType().GetProperties(), ref dic);
                }
                else if (propertie.PropertyType.IsGenericType)
                {
                    if (propertie.PropertyType.IsAssignableTo(typeof(IList)))
                    {
                        var genericType = propertie.PropertyType.GetGenericArguments().FirstOrDefault();
                        var isValueType = (genericType != null && genericType.IsValueType || genericType == typeof(string));

                        var subObj = propertie.GetValue(value);
                        if (subObj != null)
                        {
                            int count = Convert.ToInt32(subObj.GetType().GetProperty("Count").GetValue(subObj, null));
                            var itemProperty = subObj.GetType().GetProperty("Item");
                            for (int i = 0; i < count; i++)
                            {
                                object item = itemProperty.GetValue(subObj, new object[] { i });
                                if (isValueType)
                                {
                                    dic.Add($"{field}[{i}]", item.ToString());
                                }
                                else
                                {
                                    Handle($"{field}[{i}]", item, genericType.GetProperties(), ref dic);
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    /// <summary>
    /// 登录传输数据ViewModel
    /// </summary>
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VerifyCode { get; set; }
        public string CompanyName { get; set; }
        public string TokenId { get; set; }
        public string Browser { get; set; }
    }

    /// <summary>
    /// 物料中心查询参数
    /// </summary>
    public class MaterialCenterQueryModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 物料型号
        /// </summary>
        public string PN { get; set; }

        /// <summary>
        /// 批量物料型号
        /// </summary>
        public IEnumerable<string> PNList { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Top n条
        /// </summary>
        public int Top { get; set; }
        public string QueryKeywords { get; set; }
        public List<MaterialCenterQueryModel> MyProperty { get; set; }
    }

    class ModelTest
    {
        private string username { get; set; }

        public ModelTest()
        {
            this.Dicts = new Dictionary<string, ObjectTest> { };
            this.Dicts.Add("1", new ObjectTest { Age = 1, MyProperty = "DictItem1" });
            this.Dicts.Add("2", new ObjectTest { Age = 2, MyProperty = "DictItem1" });
        }

        public int[] ArrayList { get; set; } = new int[] { 1, 2, 3 };
        public ObjectTest[] ObjectTestList { get; set; } = new ObjectTest[] {
            new ObjectTest {
                Age = 1,
            }
        };

        public int Name { get; set; } = 123;
        public string NameStr { get; set; } = "sdf";
        public List<string> Lists { get; set; } = new List<string> { "1", "2" };


        public Dictionary<string, ObjectTest> Dicts { get; set; }
        public List<ObjectTest> ObjectLists { get; set; } = new List<ObjectTest> { new ObjectTest { Age = 1, MyProperty = "ListItem1" } };
        public ObjectTest ObjectInfo { get; set; } = new ObjectTest { Age = 2, MyProperty = "ObjectInfo" };
    }

    class ObjectTest
    {
        public int Age { get; set; }
        public string MyProperty { get; set; }
        //public List<ObjectTest> list { get; set; } = new List<ObjectTest> { new ObjectTest { Age = 1 } };
    }
}
