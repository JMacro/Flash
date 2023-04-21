using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace Flash.Widgets.Controllers
{
    public class VoiceBroadcastController : BaseController
    {
        [HttpPost("Microsoft")]
        public string Microsoft([FromBody] VoiceBroadcastContentRequestData requestData)
        {
            Extensions.Check.Argument.IsNotNull(requestData, nameof(requestData));
            Extensions.Check.Argument.IsNotEmpty(requestData.Content, nameof(requestData.Content));

            using (var stream = new MemoryStream())
            {
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
                    promptBuilder.AppendText(requestData.Content);
                    speechx.Speak(promptBuilder);
                    stream.Position = 0;
                }

                return $"data:audio/wav;base64,{Convert.ToBase64String(stream.ToArray())}";
#pragma warning restore CA1416 // 验证平台兼容性
            }
        }
    }

    public class VoiceBroadcastContentRequestData
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
