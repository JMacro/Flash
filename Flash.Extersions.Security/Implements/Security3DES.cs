using System;
using System.Security.Cryptography;
using System.Text;

namespace Flash.Extersions.Security
{
    public class Security3DES : ISecurity3DES
    {
        private readonly string _secretKey;
        private readonly Encoding _encoding;
        public Security3DES(string secretKey, Encoding encoding)
        {
            this._secretKey = secretKey;
            this._encoding = encoding;
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new ArgumentNullException("Secret key is null or empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Secret value is null or empty");
            }

            if (_encoding == null)
            {
                throw new ArgumentNullException("Secret encoding is null");
            }

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(_encoding.GetBytes(_secretKey));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            byte[] Buffer = Convert.FromBase64String(value);
            return _encoding.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new ArgumentNullException("Secret key is null or empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Secret value is null or empty");
            }

            if (_encoding == null)
            {
                throw new ArgumentNullException("Secret encoding is null");
            }

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(_encoding.GetBytes(_secretKey));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] Buffer = _encoding.GetBytes(value);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
    }
}
