using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using CSRedis;

namespace ConsoleApp1
{
    public class RedisUtility
    {
        private static Dictionary<string, RedisUtility> _redisDic;
        private CSRedisClient[] _rds;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbSize"></param>
        public RedisUtility(string connectionString, int dbSize)
        {
            _rds = new CSRedisClient[dbSize];
            for (int i = 0; i < dbSize; i++)
            {
                _rds[i] = new CSRedisClient(connectionString + ",defaultDatabase=" + i);
            }
        }

        /// <summary>
        /// 初始化读取配置文件
        /// </summary>
        public static void Init()
        {
            _redisDic = new Dictionary<string, RedisUtility>();
            string jsonFileName = "redis.json";
            string keys = GetConfig(jsonFileName, "serviceName", "dbName");
            string[] keySplit = keys.Split(',');

            foreach (var item in keySplit)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                string connectionString = GetConfig(jsonFileName, item, "connectionString");
                int size = Convert.ToInt32(GetConfig(jsonFileName, item, "size"));
                _redisDic[item] = new RedisUtility(connectionString, size);
            }
        }

        /// <summary>
        /// 获取项目配置文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="configKey1"></param>
        /// <param name="configKey2"></param>
        /// <returns></returns>
        private static string GetConfig(string fileName, string configKey1, string configKey2)
        {
            var fileTxt = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\config\\{fileName}");
            var file = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileTxt);

            return file[configKey1][configKey2];
        }

        #region main methods

        /// <summary>
        /// 获取具体redis库对象
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static RedisUtility GetInstance(string dbName)
        {
            return _redisDic[dbName];
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dbIndex"></param>
        /// <returns></returns>
        public T Get<T>(string key, int dbIndex)
        {
            try
            {
                return _rds[dbIndex].Get<T>(key);
            }
            catch (Exception e)
            {
                return default;
            }
        }

        #endregion
    }
}
