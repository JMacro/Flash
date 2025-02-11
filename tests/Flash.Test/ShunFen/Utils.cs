using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace Flash.Test.ShunFen
{
    public static class Utils
    {
        //String serviceCode = "EXP_RECE_CREATE_ORDER";
        //String path = "./callExpressRequest/01.order.json";//下订单

        // String serviceCode = "EXP_RECE_SEARCH_ORDER_RESP";
        // String path = "./callExpressRequest/02.order.query.json";//订单结果查询

        // String serviceCode = "EXP_RECE_UPDATE_ORDER";
        //  String path = "./callExpressRequest/03.order.confirm.json";//订单确认取消

        //   String serviceCode = "EXP_RECE_FILTER_ORDER_BSP";
        //   String path = "./callExpressRequest/04.order.filter.json";//订单筛选	

        //String serviceCode = "EXP_RECE_SEARCH_ROUTES";
        //String path = "./callExpressRequest/05.route_query_by_MailNo.json";//路由查询-通过运单号
        //String path = "./callExpressRequest/05.route_query_by_OrderNo.json";//路由查询-通过订单号

        //String serviceCode = "EXP_RECE_GET_SUB_MAILNO";
        // String path = "./callExpressRequest/07.sub.mailno.json";//子单号申请

        //  String serviceCode = "EXP_RECE_QUERY_SFWAYBILL";
        // String path = "./callExpressRequest/09.waybills_fee.json";//清单运费查询 

        //  String serviceCode = "EXP_RECE_REGISTER_ROUTE";
        // String path = "./callExpressRequest/12.register_route.json";//路由注册 

        //  String serviceCode = "EXP_RECE_CREATE_REVERSE_ORDER";
        // String path = "./callExpressRequest/13.reverse_order.json";//退货下单 

        //  String serviceCode = "EXP_RECE_CANCEL_REVERSE_ORDER";
        // String path = "./callExpressRequest/14.cancel_reverse_order.json";//退货消单

        //  String serviceCode = "EXP_RECE_DELIVERY_NOTICE";
        // String path = "./callExpressRequest/15.delivery_notice.json";//派件通知

        //  String serviceCode = "EXP_RECE_REGISTER_WAYBILL_PICTURE";
        // String path = "./callExpressRequest/16.register_waybill_picture.json";//图片注册及推送

        //  String serviceCode = "EXP_RECE_WANTED_INTERCEPT";
        // String path = "./callExpressRequest/8.wanted_intercept.json";//截单转寄

        //  String serviceCode = "COM_RECE_CLOUD_PRINT_WAYBILLS";
        // String path = "./callExpressRequest/20.cloud_print_waybills.json";//云打印面单打印

        //  String serviceCode = "EXP_RECE_UPLOAD_ROUTE";
        // String path = "./callExpressRequest/21.upload_route.json";//路由上传

        //  String serviceCode = "EXP_RECE_SEARCH_PROMITM";
        // String path = "./callExpressRequest/22.search_promitm.json";//预计派送时间

        //  String serviceCode = "EXP_EXCE_CHECK_PICKUP_TIME";
        // String path = "./callExpressRequest/23.check_pickup_time.json";//揽件服务时间

        // String serviceCode = "EXP_RECE_VALIDATE_WAYBILLNO";
        // String path = "./callExpressRequest/24.validate_waybillno.json";//运单号合法性校验

        public static JsonNode Send(string serviceCode, string msgJson)
        {
            string partnerID = "HQDZWGUPECX6";//此处替换为您在丰桥平台获取的顾客编码          
            string checkword = "aCpdqLCBeshwnv2n18SQXukbmn0fhtWV";//此处替换为您在丰桥平台获取的校验码

            string msgData = JsonCompress(msgJson);

            string timestamp = GetTimeStamp(); //获取时间戳       

            string requestID = Guid.NewGuid().ToString(); //获取uuid

            string msgDigest = MD5ToBase64String(UrlEncode(msgData + timestamp + checkword));

            Console.WriteLine("serviceCode: " + serviceCode);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("partnerID: " + partnerID);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("checkword: " + checkword);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("timestamp: " + timestamp);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("requestID: " + requestID);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("msgDigest: " + msgDigest);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("请求报文: " + msgData + timestamp + checkword);
            Console.WriteLine("--------------------------------------");

            string reqURL = "https://sfapi-sbox.sf-express.com/std/service";
            //String reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境

            string respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

            if (respJson != null)
            {
                Console.WriteLine("--------------------------------------");
                Console.WriteLine("返回报文: " + respJson);
                Console.WriteLine("--------------------------------------");
            }

            var result = JsonObject.Parse(respJson);
            if (result["apiResultCode"].GetValue<string>() != "A1000")
            {
                throw new Exception($"{result["apiResultCode"]}:{result["apiErrorMsg"]}:{result["apiResultData"]}");
            }

            var apiResultDataStr = result["apiResultData"].GetValue<string>();

            return GetApiResultData(apiResultDataStr);
        }

        public static JsonNode GetApiResultData(string apiResultData)
        {
            return JsonObject.Parse(apiResultData);
        }

        private static string JsonCompress(string msgData)
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader reader = new StringReader(msgData))
            {
                int ch = -1;
                int lastch = -1;
                bool isQuoteStart = false;
                while ((ch = reader.Read()) > -1)
                {
                    if ((char)lastch != '\\' && (char)ch == '\"')
                    {
                        if (!isQuoteStart)
                        {
                            isQuoteStart = true;
                        }
                        else
                        {
                            isQuoteStart = false;
                        }
                    }
                    if (!char.IsWhiteSpace((char)ch) || isQuoteStart)
                    {
                        sb.Append((char)ch);
                    }
                    lastch = ch;
                }
            }
            return sb.ToString();
        }

        private static string callSfExpressServiceByCSIM(string reqURL, string partnerID, string requestID, string serviceCode, string timestamp, string msgDigest, string msgData)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            Dictionary<string, string> content = new Dictionary<string, string>();
            content["partnerID"] = partnerID;
            content["requestID"] = requestID;
            content["serviceCode"] = serviceCode;
            content["timestamp"] = timestamp;
            content["msgDigest"] = msgDigest;
            content["msgData"] = msgData;

            if (!(content == null || content.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in content.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, content[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, content[key]);
                    }
                    i++;
                }

                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private static string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        private static string MD5ToBase64String(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] MD5 = md5.ComputeHash(Encoding.UTF8.GetBytes(str));//MD5(注意UTF8编码)
            string result = Convert.ToBase64String(MD5);//Base64
            return result;
        }

        public static string Read(string fileName)
        {
            StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "ShunFen", "Config", fileName), Encoding.UTF8);

            StringBuilder builder = new StringBuilder();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                builder.Append(line);
            }
            return builder.ToString();
        }

        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
