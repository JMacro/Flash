using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

namespace Flash.Extensions.Email
{
    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 附件内容
        /// </summary>
        public Stream FileStream { get; set; }
        /// <summary>
        /// MainMimeType
        /// </summary>
        public string MainMimeType { get; set; }
        /// <summary>
        /// SubMimeType
        /// </summary>
        public string SubMimeType { get; set; }

        /// <summary>
        /// 创建附件
        /// </summary>
        /// <param name="fileName">附件名称</param>
        /// <param name="fileStream">附件内容</param>
        /// <param name="mimeType">MimeType</param>
        /// <returns></returns>
        public static AttachmentInfo Create(string fileName, Stream fileStream, string mimeType = MediaTypeNames.Application.Octet)
        {
            var tempMimeType = mimeType.Split('/');
            return new AttachmentInfo
            {
                FileName = fileName,
                FileStream = fileStream,
                MainMimeType = tempMimeType[0],
                SubMimeType = tempMimeType[1]
            };
        }

        /// <summary>
        /// 创建附件
        /// </summary>
        /// <param name="filePath">附件路径</param>
        /// <returns></returns>
        public static AttachmentInfo Create(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件不存在[{filePath}]");
            }
            return Create(Path.GetFileName(filePath), File.OpenRead(filePath), MimeTypes.GetMimeType(filePath));
        }

        /// <summary>
        /// 创建附件
        /// </summary>
        /// <param name="filePaths">附件路径</param>
        /// <returns></returns>
        public static List<AttachmentInfo> Create(params string[] filePaths)
        {
            Check.Argument.IsNotEmpty(filePaths, nameof(filePaths));

            var result = new List<AttachmentInfo>();
            foreach (var filePath in filePaths)
            {
                result.Add(Create(filePath));
            }
            return result;
        }
    }
}
