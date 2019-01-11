using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// DB工厂
    /// </summary>
    public static class SessionFactory
    {
        private const string key = "95C31F7CDC9FEFB0";

        /// <summary>
        /// 加密连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static string EncryptConnectionString(string connectionString)
        {
            return Cryptographer.DES3Encrypt(connectionString, key);
        }

        public static string DecryptConnectionString(string encryptString)
        {
            return Cryptographer.DES3Decrypt(encryptString, key);
        }

        /// <summary>
        /// 创建session
        /// </summary>
        /// <returns></returns>
        public static ISession GetSession()
        {
            string providerName = ConfigurationManager.ConnectionStrings[0].ProviderName;
            string connectionString = Cryptographer.DES3Decrypt(ConfigurationManager.ConnectionStrings[0].ConnectionString, key);

            return GetSession(providerName, connectionString);
        }

        /// <summary>
        /// 创建session
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ISession GetSession(string name)
        {
            string providerName = ConfigurationManager.ConnectionStrings[name].ProviderName;
            string connectionString = Cryptographer.DES3Decrypt(ConfigurationManager.ConnectionStrings[name].ConnectionString, key);

            return GetSession(providerName, connectionString);
        }

        /// <summary>
        /// 创建session
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ISession GetSession(string providerName, string connectionString)
        {
            switch (providerName)
            {
                case "MySql.Data":
                    return new MySqlProvider(connectionString);
                default:
                    return null;
            }
        }
    }
}
