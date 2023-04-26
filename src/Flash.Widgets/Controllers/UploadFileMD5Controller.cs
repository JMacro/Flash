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
            using (var stream = new MemoryStream())
            {
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
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ReadProperty")]
        public async Task<BaseDataResponse<object>> ReadProperty([FromBody] ReadPropertyRequestData request)
        {
            Check.Argument.IsNotEmpty(request.FileName, nameof(request.FileName));
            var data = new List<IExifValue>();
            var filePath = Path.Combine(FilePath, request.FileName);
            var defaultExifTag = ExifTagDescriptionRequestData.CreateDefault();

            using (var image = await Image.LoadAsync(filePath))
            {
                var exif = image.Metadata.ExifProfile;
                var list = defaultExifTag.Select(p =>
                {
                    var imageExif = exif.Values.FirstOrDefault(f => f.Tag == p.Tag);
                    if (imageExif == null)
                    {
                        return new
                        {
                            Key = p.Tag.ToString(),
                            Value = "",
                            Description = p.Description
                        };
                    }

                    var obj = new
                    {
                        Key = imageExif.Tag.ToString(),
                        Value = GetExifValueAsString(imageExif),
                        Description = p.Description
                    };
                    return obj;
                }).ToList();

                return BaseDataResponse<object>.Create(0, data: list);
            }
        }

        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("WriteProperty")]
        public async Task<BaseDataResponse<object>> WriteProperty([FromBody] WritePropertyRequestData request)
        {
            Check.Argument.IsNotEmpty(request.FileName, nameof(request.FileName));
            Check.Argument.IsNotEmpty(request.Propertys, nameof(request.Propertys));

            var data = new List<IExifValue>();
            var filePath = Path.Combine(FilePath, request.FileName);
      
            using (Image image = await Image.LoadAsync(filePath))
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
                image.Save(filePath);
            }

            return BaseDataResponse<object>.Create(0);

        }

        public static string GetExifValueAsString(IExifValue exifValue)
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
