using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Utility
{
    /// <summary>
    /// 加解密方法
    /// </summary>
    public static class Cryptographer
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Encrypt(string original)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(original, "MD5");
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string original)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(original, "SHA1");
        }

        /// <summary>
        /// DES3加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Encrypt(string original, string key)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
                }

                des.Mode = CipherMode.ECB;

                byte[] buffer = Encoding.Unicode.GetBytes(original);

                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }

        /// <summary>
        /// DES3解密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DES3Decrypt(string original, string key)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
                }

                des.Mode = CipherMode.ECB;

                byte[] buffer = Convert.FromBase64String(original);

                return Encoding.Unicode.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }
    }
}
