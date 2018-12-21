using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class EncryptHelper
    {
        public static readonly string DES3Key = ConfigurationManager.AppSettings["des3_key"].ToString();
        public static readonly string AESKey = ConfigurationManager.AppSettings["aes128_key"].ToString();

        #region 3DES

        /// <summary>
        /// 加密明文字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DES3Encrypt(this string data)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Encoding.UTF8.GetBytes(DES3Key);

                des.Mode = CipherMode.ECB;

                byte[] buffer = Encoding.UTF8.GetBytes(data);

                return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }

        /// <summary>
        /// 3DES 加密
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DES3Encrypt(this object obj)
        {
            var clearData = obj.Serialize();

            return DES3Encrypt(clearData);
        }

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <param name="secretData"></param>
        /// <returns></returns>
        public static string DES3Decrypt(this string secretData)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Encoding.UTF8.GetBytes(DES3Key);

                des.Mode = CipherMode.ECB;

                byte[] buffer = Convert.FromBase64String(secretData);

                return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
            }
        }

        /// <summary>
        /// 3DES 解密
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="secretData"></param>
        /// <returns></returns>
        public static TModel DES3Decrypt<TModel>(this string secretData)
            where TModel : class
        {
            var clearText = DES3Decrypt(secretData);

            return JsonConvertExtension.Deserialize<TModel>(clearText);
        }

        #endregion

        #region AES

        /// <summary>
        /// AES加密 (128-ECB加密模式)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string AESEncrypt(this string data)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(AESKey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);

            RijndaelManaged rDel = new RijndaelManaged();

            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密 (128-ECB加密模式)
        /// </summary>
        /// <param name="secretData"></param>
        /// <returns></returns>
        public static string AESDencrypt(this string secretData)
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(AESKey);
                byte[] toDecryptArray = Convert.FromBase64String(secretData);

                RijndaelManaged rDel = new RijndaelManaged();

                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string AESEncrypt(this object obj)
        {
            var clearText = obj.Serialize();

            return AESEncrypt(clearText);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="secretData"></param>
        /// <returns></returns>
        public static TData AESDencrypt<TData>(this string secretData)
            where TData : class
        {
            var clearText = secretData.AESDencrypt();
            return JsonConvertExtension.Deserialize<TData>(clearText);
        }

        #endregion

        #region MD5

        ///// <summary>
        ///// MD5加密
        ///// </summary>
        ///// <param name="clearText"></param>
        ///// <returns></returns>
        //public static string MD5Encrypt(this string clearText)
        //{
        //    byte[] result = Encoding.UTF8.GetBytes(clearText);

        //    MD5 md5 = new MD5CryptoServiceProvider();

        //    byte[] output = md5.ComputeHash(result);

        //    return BitConverter.ToString(output).Replace("-", "").ToLower();
        //}

        #endregion

        #region 可解密的MD5

        /// <summary>
        /// 检查密码是否正确
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        public static bool CheckPswd(string userPswd, string pass)
        {
            return pass == EncryptHelper.MD5Decrypt(userPswd);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this string password)
        {
            string key = GenerateKey();
            var pswd = MD5Encrypt(password, key);
            return pswd + ";" + key;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Decrypt(this string password)
        {
            var pswd = password.Split(';');
            return MD5Decrypt(pswd[0], pswd[1]);
        }

        // 创建Key
        private static string GenerateKey()
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
        ///MD5加密
        private static string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();


        }

        ///MD5解密
        private static string MD5Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        #endregion
    }
}
