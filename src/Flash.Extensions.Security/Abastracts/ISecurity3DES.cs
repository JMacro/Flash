using System;
using System.Collections.Generic;
using System.Text;

namespace Flash.Extensions.Security
{
    public interface ISecurity3DES
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Encrypt(string value);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Decrypt(string value);
    }
}
