using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Flash.Extensions
{
    public class BusinessException : Exception
    {
        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// 异常抛出
        /// </summary>
        /// <param name="throwMessage"></param>
        /// <exception cref="BusinessException"></exception>
        public static void Throw(string throwMessage = "非法请求")
        {
            throw new BusinessException(throwMessage);
        }
    }
}
