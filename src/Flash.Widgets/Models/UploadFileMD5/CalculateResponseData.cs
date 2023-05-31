namespace Flash.Widgets.Models.UploadFileMD5
{
    public class CalculateResponseData
    {
        /// <summary>
        /// Exif信息
        /// </summary>
        public List<ReadPropertyResponseData> ExifInfoList { get; set; }
        /// <summary>
        /// 文件身份
        /// </summary>
        public CalculateResponseData4FileIdentity FileIdentityInfo { get; set; }
    }

    public class CalculateResponseData4FileIdentity
    {
        /// <summary>
        /// 哈希值
        /// </summary>
        public string Hash { get; set; }
        public string CRC32 { get; set; }
        public string CRC64 { get; set; }
        public string SHA256 { get; set; }
    }
}
