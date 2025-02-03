using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Flash.Widgets.Models.UploadFileMD5
{
    public class ExifTagDescriptionRequestData
    {
        public string Tag { get; set; }
        public string Description { get; set; }

        public static ExifTagDescriptionRequestData Create(ExifTag tag, string description)
        {
            return new ExifTagDescriptionRequestData
            {
                Tag = tag.ToString(),
                Description = description
            };
        }

        public static List<ExifTagDescriptionRequestData> CreateDefault()
        {
            return new List<ExifTagDescriptionRequestData> {
                Create(ExifTag.DateTimeOriginal, "拍摄时间"),
                Create(ExifTag.Make, "相机制造商"),
                Create(ExifTag.Model, "相机型号"),
                Create(ExifTag.Software, "拍摄设备软件版本"),
                Create(ExifTag.ISOSpeedRatings, "ISO感光度"),
                Create(ExifTag.ExposureTime, "曝光时间"),
                Create(ExifTag.FNumber, "光圈值"),
                Create(ExifTag.FocalLength, "焦距"),
                Create(ExifTag.Flash, "闪光灯使用状态"),
                Create(ExifTag.WhiteBalance, "白平衡模式"),
                Create(ExifTag.XResolution, "水平分辨率"),
                Create(ExifTag.YResolution, "垂直分辨率"),
                Create(ExifTag.Artist, "作者")
            };
        }
    }
}
