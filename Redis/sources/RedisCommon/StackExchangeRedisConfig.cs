using StackExchange.Redis;
using System.Configuration;

namespace Jiajue.BeiJi.Redis.RedisCommon
{
    /// <summary>
    /// 配置类
    /// </summary>
    public static class StackExchangeRedisConfig
    {
        private static readonly string config = ConfigurationManager.AppSettings["redis_config"];
        private static readonly string pwd = ConfigurationManager.AppSettings["redis_pwd"];
        private static readonly string key = ConfigurationManager.AppSettings["redis_key"] ?? "";

        public static readonly ConfigurationOptions Option = new ConfigurationOptions()
        {
            EndPoints = { config },//可多个
            Password = pwd,
            DefaultDatabase=1
            //ConnectTimeout = 1000,//连接操作超时
            //KeepAlive = 180,//发送消息以保住保持套接字活动时间
            //SyncTimeout = 2000,//允许进行同步操作
            //ConnectRetry=3 //重试连接的次数
        };

        public static string Key()
        {
            return key;
        }
    }
}
