using ServiceStack.OrmLite;
using System;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Utility.DbConnection
{
    /// <summary>
    /// 数据库基本连接
    /// </summary>
    public class BaseDataAccess
    {
        #region 数据库配置

        public BaseDataAccess()
        {
            OrmLiteConfig.DialectProvider = MySqlDialect.Provider;
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection
        {
            get
            {
                return DbFactory.OpenDbConnection();
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                return DataAccessEncrypt.AESDencrypt(ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString);
            }
        }

        private OrmLiteConnectionFactory conFactory;
        /// <summary>
        /// 连接工厂
        /// </summary>
        public OrmLiteConnectionFactory DbFactory
        {
            get
            {
                if (null == conFactory)
                {
                    conFactory = new OrmLiteConnectionFactory(ConnectionString, MySqlDialect.Provider);
                }
                return conFactory;
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DataAccessEncrypt
    {
        private static readonly string AESKey = "fc0da45737d14c1a";

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
    }
}
