using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;

namespace Flash.Test.ShunFen
{
    [TestFixture]
    public class ShunFenTests
    {
        public List<string> waybillNoList = new List<string>();

        /// <summary>
        /// 下订单
        /// </summary>
        [Test]
        public void A01_EXP_RECE_CREATE_ORDER()
        {
            string msgJson = Utils.Read("01.EXP_RECE_CREATE_ORDER.json");
            var jsonObject = JsonObject.Parse(msgJson);
            jsonObject["orderId"] = $"JMacro_{DateTime.Now.Ticks}";
            var response = Utils.Send("EXP_RECE_CREATE_ORDER", jsonObject.ToJsonString());
            var waybillNoInfoList = response["msgData"]["waybillNoInfoList"] as JsonArray;
            waybillNoList = waybillNoInfoList.Select(p => p["waybillNo"].GetValue<string>()).ToList();
            A02_COM_RECE_CLOUD_PRINT_WAYBILLS(waybillNoList);
        }

        /// <summary>
        /// 云打印面单
        /// </summary>
        [Test]
        [TestCase(null)]
        public void A02_COM_RECE_CLOUD_PRINT_WAYBILLS(List<string> waybillNoList = null)
        {
            var msgJson = Utils.Read("25.COM_RECE_CLOUD_PRINT_WAYBILLS.json");
            var request = msgJson;
            if (waybillNoList != null && waybillNoList.Any())
            {
                var jsonObject = JsonObject.Parse(msgJson);
                var documents = (jsonObject["documents"] as JsonArray);
                documents.Clear();
                foreach (var item in waybillNoList)
                {
                    documents.Add(new { masterWaybillNo = item });
                }
                request = jsonObject.ToJsonString();
            }
            var response = Utils.Send("COM_RECE_CLOUD_PRINT_WAYBILLS", request);
            var files = response["obj"]["files"] as JsonArray;
            var datas = files.Select(p => new
            {
                Token = p["token"].GetValue<string>(),
                Url = p["url"].GetValue<string>(),
                WaybillNo = p["waybillNo"].GetValue<string>(),
            }).ToList();

            var savePath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "ShunFen", "Download"));
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            foreach (var item in datas)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(item.Url);
                req.Method = "GET";

                req.Headers.Add("X-Auth-token", item.Token);

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (Stream stream = resp.GetResponseStream())
                {
                    using (var fileStream = File.Create(Path.Combine(savePath, $"{item.WaybillNo}.pdf")))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }
    }
}
