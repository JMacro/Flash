using Flash.Extensions;
using Flash.Widgets.Models;
using Flash.Widgets.Models.UploadFileMD5;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats;
using System.Security.Cryptography;
using System.Text;

namespace Flash.Widgets.Controllers
{
    public class UploadFileMD5Controller : BaseController
    {
        private readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "uploadImg");

        public UploadFileMD5Controller()
        {
        }


        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("Calculate")]
        public async Task<BaseDataResponse<CalculateResponseData>> Calculate(IFormFile formFile)
        {
            Check.Argument.IsNotNull(formFile, nameof(formFile));
            var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            var buffer = stream.GetBuffer();
            var calculateValue = "";
            using (var md5 = MD5.Create())
            {
                var md5bt = md5.ComputeHash(buffer);
                //将byte数组转换为字符串
                StringBuilder builder = new StringBuilder();
                foreach (var item in md5bt)
                {
                    builder.Append(item.ToString("X2"));
                }
                calculateValue = builder.ToString();
            }

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            var fileName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(formFile.FileName);
            var path = Path.Combine(FilePath, fileName);
            await System.IO.File.WriteAllBytesAsync(path, buffer);
            return BaseDataResponse<CalculateResponseData>.Create(0, data: new CalculateResponseData
            {
                Hash = calculateValue,
                FileName = fileName
            });
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ReadProperty")]
        public async Task<BaseDataResponse<Dictionary<string, string>>> ReadProperty([FromBody] ReadPropertyRequestData request)
        {
            Check.Argument.IsNotEmpty(request.FileName, nameof(request.FileName));
            var data = new Dictionary<string, string>();
            FileInfo fileInfo = new FileInfo(Path.Combine(FilePath, request.FileName));
            using (var stream = fileInfo.OpenRead())
            {
                if (IsImage(stream))
                {
                    Image image = await Image.LoadAsync(stream);
                    var d = image.Metadata;
                }
            }
            return BaseDataResponse<Dictionary<string, string>>.Create(0, data: data);
        }

        private bool IsImage(Stream stream)
        {
            IImageFormat format;
            try
            {
                format = Image.DetectFormat(stream);
            }
            catch (Exception)
            {
                // 文件不是图片格式，返回 false
                return false;
            }

            // 判断文件格式是否为图片格式
            return format != null;
        }
    }
}
