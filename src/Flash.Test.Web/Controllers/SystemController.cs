using Flash.Test.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;
using System.IO;
using System.Speech.AudioFormat;
using System.Reflection;

namespace Flash.Test.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        public SystemController()
        {
        }

        [HttpGet("Test1")]
        public object Test1()
        {
            var osVersion = System.Environment.OSVersion;
            return new
            {
                UseMemorySize = Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024,
                StartTime = Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                WorkingSet = Process.GetCurrentProcess().WorkingSet64,
                OS = osVersion.Platform.ToString()
            };
        }

        [HttpGet("Test2")]
        public object Test2()
        {
            var data = new TestDesensitization
            {
                UserName = "sdfasdfasdf",
                Password = "123123",
                MyProperty = new TestDesensitizationSub
                {
                    UserName = "dfe3434"
                },
                BB = 123
            };


            //var data = new Dictionary<string, string>();
            //data.Add("aaa", "12313");

            //var data = new List<TestDesensitization>();
            //data.Add(
            //    new TestDesensitization
            //    {
            //        UserName = "sdfasdfasdf",
            //        Password = "123123",
            //        MyProperty = new TestDesensitizationSub
            //        {
            //            UserName = "dfe3434"
            //        },
            //        BB = 123
            //    }
            //    );

            var er = JsonConvert.SerializeObject((object)data, new JsonSerializerSettings
            {
                Converters = new[] { new DesensitizationConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver(),
            });


            return data;
        }

        [HttpGet("Test3")]
        public string Test3(string txt)
        {
            var voiceName = "Microsoft Huihui Desktop";

            txt = txt ?? "业务员在进行客户查重时，一般查到系统存在重复记录后则不再跟进，但其实客户可能是在公海，业务员可以从公海将其领走，因此应在查询页面进行提示";

            using (SpeechSynthesizer speechx = new SpeechSynthesizer())
            {
                using (var stream = new MemoryStream())
                {
                    stream.Position = 0;

                    var mi = speechx.GetType().GetMethod("SetOutputStream", BindingFlags.Instance | BindingFlags.NonPublic);
                    var fmt = new SpeechAudioFormatInfo(8000, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
                    mi.Invoke(speechx, new object[] { stream, fmt, true, true });
                    speechx.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);
                    speechx.Speak(txt);

                    //return File(stream.ToArray(), "application/octet-stream", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav");

                    return $"data:audio/wav;base64,{Convert.ToBase64String(stream.ToArray())}";
                }
            }
        }

        [HttpGet("Test4")]
        public string Test4(string txt)
        {
            return OutPutStrStream(new List<string> { "业务员在进行客户查重时，一般查到系统存在重复记录后则不再跟进", "但其实客户可能是在公海，业务员可以从公海将其领走，因此应在查询页面进行提示" });
        }

        /// <summary>
        /// 语音播报
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="txt"></param>
        private void VoiceBroadcast(MemoryStream stream, List<string> txt)
        {
            if (!txt.Any())
                return;
#pragma warning disable CA1416 // 验证平台兼容性
            using (SpeechSynthesizer speechx = new SpeechSynthesizer())
            {
                #region 语音设置
                speechx.Volume = 10;
                speechx.Rate = -5;//-10-10,值越小，语速越慢
                var fmt = new SpeechAudioFormatInfo(8000, AudioBitsPerSample.Sixteen, AudioChannel.Mono);
                #endregion

                var mi = speechx.GetType().GetMethod("SetOutputStream", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(speechx, new object[] { stream, fmt, true, true });
                speechx.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult);

                //speechx.SetOutputToAudioStream(stream, fmt);
                //将合成器配置为输出到音频流。  
                //speechx.SetOutputToWaveStream(stream);
                PromptBuilder promptBuilder = new PromptBuilder();
                txt.ToList().ForEach(x => promptBuilder.AppendText(x));
                speechx.Speak(promptBuilder);
                stream.Position = 0;
            }
#pragma warning restore CA1416 // 验证平台兼容性
        }

        /// <summary>
        /// 输出字符文件流
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private string OutPutStrStream(List<string> txt)
        {
            using (var stream = new MemoryStream())
            {
                VoiceBroadcast(stream, txt);
                return $"data:audio/wav;base64,{Convert.ToBase64String(stream.ToArray())}";
            }
        }

        /// <summary>
        /// 文字转换mp3格式音频
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="input">输入文本</param>
        /// <returns></returns>
        private static bool TextVonvertToMP3(string path, string input)
        {
            input = input.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                using (SpeechSynthesizer reader = new SpeechSynthesizer())
                {
                    reader.SetOutputToWaveFile(path + input + ".mp3");
                    reader.Speak(input);
                    reader.SetOutputToDefaultAudioDevice();
                    reader.Dispose();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 文字在线音频朗读
        /// </summary>
        /// <param name="readText">朗读文本</param>
        /// <returns></returns>
        public static bool TextRead(string readText)
        {
            var flag = false;
            readText = readText.Trim();
            if (!string.IsNullOrWhiteSpace(readText))
            {
                using (SpeechSynthesizer reader = new SpeechSynthesizer())
                {
                    reader.Speak(readText);
                    reader.Dispose();
                    flag = true;
                }
                return flag;
            }
            else
            {
                return flag;
            }
        }
    }
}
