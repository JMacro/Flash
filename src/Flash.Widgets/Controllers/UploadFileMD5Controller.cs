using Flash.Extensions;
using Flash.Widgets.Models;
using Flash.Widgets.Models.UploadFileMD5;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using System.Security.Cryptography;
using System.Text;

namespace Flash.Widgets.Controllers
{
    public class UploadFileMD5Controller : BaseController
    {
        private readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "uploadImg");
        private readonly ILogger<UploadFileMD5Controller> _logger;

        public UploadFileMD5Controller(ILogger<UploadFileMD5Controller> logger)
        {
            this._logger = logger;
        }


        /// <summary>
        /// 获得图片属性信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("GetImagePropertyInfo")]
        public async Task<BaseDataResponse<CalculateResponseData>> GetImagePropertyInfo(IFormFile file)
        {
            Check.Argument.IsNotNull(file, nameof(file));
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var buffer = stream.GetBuffer();
                var list = ReadImageExif(buffer);

                var fileIdentity = new CalculateResponseData4FileIdentity()
                {
                    Hash = CalculateHash(buffer),
                    CRC32 = CalculateCRC32(buffer),
                    CRC64 = CalculateCRC64(buffer),
                    SHA256 = CalculateSHA256(buffer),
                };
                return BaseDataResponse<CalculateResponseData>.Create(0, data: new CalculateResponseData
                {
                    ExifInfoList = list,
                    FileIdentityInfo = fileIdentity
                });
            }
        }

        /// <summary>
        /// 获得图片ExifTag列表数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetExifTagList")]
        public BaseDataResponse<List<ExifTagDescriptionRequestData>> GetExifTagList()
        {
            return BaseDataResponse<List<ExifTagDescriptionRequestData>>.Create(0, data: ExifTagDescriptionRequestData.CreateDefault());
        }

        /// <summary>
        /// 转换Base64
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("ConvertBase64")]
        public async Task<BaseDataResponse<ConvertBase64ResponseData>> ConvertBase64(IFormFile formFile)
        {
            Check.Argument.IsNotNull(formFile, nameof(formFile));
            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                var buffer = stream.GetBuffer();
                string base64Str = Convert.ToBase64String(buffer, 0, buffer.Length);
                return BaseDataResponse<ConvertBase64ResponseData>.Create(0, data: new ConvertBase64ResponseData
                {
                    Base64Str = base64Str
                });
            }
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("ReadProperty")]
        public async Task<BaseDataResponse<List<ReadPropertyResponseData>>> ReadProperty(IFormFile formFile)
        {
            Check.Argument.IsNotNull(formFile, nameof(formFile));
            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                var buffer = stream.GetBuffer();
                var list = ReadImageExif(buffer);
                return BaseDataResponse<List<ReadPropertyResponseData>>.Create(0, data: list);
            }
        }

        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("WriteProperty")]
        public async Task<BaseDataResponse<WritePropertyResponseData>> WriteProperty([FromBody] WritePropertyRequestData request)
        {
            Check.Argument.IsNotEmpty(request.Image4Base64, nameof(request.Image4Base64));
            Check.Argument.IsNotEmpty(request.Propertys, nameof(request.Propertys));

            var result = new WritePropertyResponseData();

            var prefix = "";
            if (request.Image4Base64.Contains(","))
            {
                var tmp = request.Image4Base64.Split(',');
                prefix = tmp[0];
                request.Image4Base64 = tmp[1];
            }

            var buffer = Convert.FromBase64String(request.Image4Base64);

            using (Image image = Image.Load(buffer))
            {
                var exif = image.Metadata.ExifProfile;
                foreach (var item in request.Propertys)
                {
                    var exifInfo = exif.Values.FirstOrDefault(p => p.Tag.ToString() == item.Key);
                    if (exifInfo != null)
                    {
                        exifInfo.TrySetValue(item.Value);
                    }
                }

                using (var stream = new MemoryStream())
                {
                    await image.SaveAsJpegAsync(stream);

                    //通过往源文件末尾添加uuid二进制数据来改变原文件的MD5值
                    var uuidBuffer = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));
                    await stream.WriteAsync(uuidBuffer, 0, uuidBuffer.Length);

                    var imageBuffer = stream.GetBuffer();
                    var calculateValue = CalculateHash(imageBuffer);
                    result.Image4Base64 = string.Join(',', prefix, Convert.ToBase64String(imageBuffer));
                    result.FileIdentityInfo = new CalculateResponseData4FileIdentity()
                    {
                        Hash = CalculateHash(imageBuffer),
                        CRC32 = CalculateCRC32(imageBuffer),
                        CRC64 = CalculateCRC64(imageBuffer),
                        SHA256 = CalculateSHA256(imageBuffer),
                    };
                }
            }

            return BaseDataResponse<WritePropertyResponseData>.Create(0, data: result);
        }

        /// <summary>
        /// 计算Hash值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string CalculateHash(byte[] buffer)
        {
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
            return calculateValue;
        }

        /// <summary>
        /// 计算CRC32值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string CalculateCRC32(byte[] buffer)
        {
            return string.Join(string.Empty, new Masuit.Tools.Security.Crc32().ComputeHash(buffer).Select(b => b.ToString("X2")));
        }

        /// <summary>
        /// 计算CRC64值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string CalculateCRC64(byte[] buffer)
        {
            return string.Join(string.Empty, new Masuit.Tools.Security.Crc64().ComputeHash(buffer).Select(b => b.ToString("X2")));
        }

        /// <summary>
        /// 计算SHA256值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string CalculateSHA256(byte[] buffer)
        {
            using var sha256 = SHA256.Create();
            byte[] result = sha256.ComputeHash(buffer);
            return Convert.ToBase64String(result); //返回长度为44字节的字符串
        }

        /// <summary>
        /// 读取图片Exif信息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private List<ReadPropertyResponseData> ReadImageExif(byte[] buffer)
        {
            var defaultExifTag = ExifTagDescriptionRequestData.CreateDefault();
            using (var image = Image.Load(buffer))
            {
                var exif = image.Metadata.ExifProfile;
                var list = new List<ReadPropertyResponseData>();
                foreach (var p in defaultExifTag)
                {
                    var imageExif = exif.Values.FirstOrDefault(f => f.Tag.ToString() == p.Tag);
                    if (imageExif == null)
                    {
                        list.Add(new ReadPropertyResponseData
                        {
                            Key = p.Tag,
                            Value = "",
                            Description = p.Description
                        });
                    }
                    else
                    {
                        list.Add(new ReadPropertyResponseData
                        {
                            Key = p.Tag,
                            Value = GetExifValueAsString(imageExif),
                            Description = p.Description
                        });
                    }

                }
                return list;
            }
        }

        private static string GetExifValueAsString(IExifValue exifValue)
        {
            switch (exifValue.DataType)
            {
                case ExifDataType.Ascii:
                case ExifDataType.Undefined:
                    return exifValue.GetString();
                case ExifDataType.Byte:
                    return exifValue.GetByteArray()?.ToString();
                case ExifDataType.Short:
                    return exifValue.GetShort()?.ToString();
                case ExifDataType.Long:
                    return exifValue.GetLong()?.ToString();
                case ExifDataType.Rational:
                    return exifValue.GetRational()?.ToString();
                case ExifDataType.SignedByte:
                    return exifValue.GetSByte().ToString();
                case ExifDataType.SignedShort:
                    return exifValue.GetSShort()?.ToString();
                case ExifDataType.SignedLong:
                    return exifValue.GetSLong()?.ToString();
                case ExifDataType.SignedRational:
                    return exifValue.GetSRational()?.ToString();
                case ExifDataType.SingleFloat:
                    return exifValue.GetSingle()?.ToString();
                case ExifDataType.DoubleFloat:
                    return exifValue.GetDouble().ToString();
                default:
                    return null;
            }

        }
    }

    public static class HH
    {
        public static string GetString(this IExifValue exifValue)
        {
            if (exifValue == null)
            {
                return string.Empty;
            }

            var value = exifValue.GetValue();

            if (exifValue.IsArray)
            {
                return Encoding.UTF8.GetString(value as byte[]);
            }


            return value?.ToString() ?? string.Empty;
        }

        public static double GetDouble(this IExifValue exifValue)
        {
            if (exifValue == null)
            {
                return 0;
            }

            var value = exifValue.GetValue();
            if (double.TryParse(value?.ToString(), out double result))
            {
                return result;
            }

            return 0;
        }

        public static sbyte GetSByte(this IExifValue exifValue)
        {
            if (exifValue == null)
            {
                return 0;
            }

            var value = exifValue.GetValue();
            if (sbyte.TryParse(value?.ToString(), out sbyte result))
            {
                return result;
            }

            return 0;
        }

        public static string GetByteArray(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (value == null)
                return null;

            byte[] bytes = (byte[])value;
            return string.Join(" ", bytes);
        }

        public static string GetShort(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.Short || value == null)
                return null;

            if (exifValue.IsArray)
            {

            }

            return value.ToString();
        }

        public static string GetLong(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.Long || value == null)
                return null;

            return Convert.ToUInt32(value).ToString();
        }

        public static string GetRational(this IExifValue exifValue)
        {
            var value = exifValue.GetValue() as Rational?;

            if (exifValue.DataType != ExifDataType.Rational || value == null)
                return null;

            return value?.ToSingle().ToString();
        }

        public static string GetSShort(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.SignedShort || value == null)
                return null;

            return Convert.ToInt16(value).ToString();
        }

        public static string GetSLong(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.SignedLong || value == null)
                return null;

            return Convert.ToInt32(value).ToString();
        }

        public static string GetSRational(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.SignedRational || value == null)
                return null;

            int[] srational = (int[])value;
            return $"{srational[0]}/{srational[1]}";
        }

        public static string GetSingle(this IExifValue exifValue)
        {
            var value = exifValue.GetValue();
            if (exifValue.DataType != ExifDataType.SingleFloat || value == null)
                return null;

            return Convert.ToSingle(value).ToString();
        }

    }


}
