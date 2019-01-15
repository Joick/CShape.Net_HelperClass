using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.RedisCommon
{
    /// <summary>
    /// 基础类
    /// </summary>
    public class StackExchangeRedisBase
    {
        private static ConnectionMultiplexer db = StackExchangeRedisManager.Instance;
        private static string key = string.Empty;

        #region Helper Methods

        /// <summary>
        /// 添加名称
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        public string AddKey(string old)
        {
            var fixkey = key ?? StackExchangeRedisConfig.Key();
            return fixkey + old;
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public T DoSave<T>(Func<IDatabase, T> func)
        {
            return func(db.GetDatabase());
        }

        /// <summary>
        /// 对象转json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public string ConvertJson<T>(T val)
        {
            return val is string ? val.ToString() : JsonConvert.SerializeObject(val);
        }

        /// <summary>
        /// 值转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public T ConvertObj<T>(RedisValue val)
        {
            return JsonConvert.DeserializeObject<T>(val);
        }

        /// <summary>
        /// 集合值转集合对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public List<T> ConvertList<T>(RedisValue[] val)
        {
            List<T> result = new List<T>();
            foreach (var item in val)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 集合转key
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public RedisKey[] ConvertRedisKeys(List<string> val)
        {
            return val.Select(k => (RedisKey)k).ToArray();
        }

        /// <summary>
        /// 删除满足条件的Redis数据
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveByPattern(string pattern)
        {
            var _muxer = StackExchangeRedisManager.Instance;
            var _db = db.GetDatabase();
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: pattern, database: _db.Database);
                foreach (var keyName in keys)
                    _db.KeyDelete(keyName);
            }
        }

        public List<T> GetListAllData<T>(string key)
        {
            //ListRange返回的是一组字符串对象
            //需要逐个反序列化成实体
            var _db = db.GetDatabase();
            var vList = _db.ListRange(key);

            List<T> result = new List<T>();

            foreach (var item in vList)
            {
                var model = ConvertObj<T>(item); //反序列化
                result.Add(model);
            }
            return result;
        }

        public async Task<List<T>> GetListAllDataAsync<T>(string key)
        {
            //ListRange返回的是一组字符串对象
            //需要逐个反序列化成实体
            var _db = db.GetDatabase();
            var vList = _db.ListRangeAsync(key);

            List<T> result = new List<T>();

            foreach (var item in await vList)
            {
                var model = ConvertObj<T>(item); //反序列化
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 获取满足条件的Redis数据
        /// </summary>
        /// <param name="pattern"></param>
        public List<T> GetStringByPattern<T>(string pattern)
        {
            List<T> result = new List<T>();
            var _muxer = StackExchangeRedisManager.Instance;
            var _db = db.GetDatabase();
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: pattern, database: _db.Database);
                foreach (var keyName in keys)
                {
                    var val = _db.StringGet(keyName);
                    if (val.HasValue)
                        result.Add(ConvertObj<T>(val));
                }
            }

            return result;
        }

        /// <summary>
        /// 获取满足条件的Redis数据
        /// </summary>
        /// <param name="pattern"></param>
        public async Task<List<List<T>>> GetRightListPattern<T>(string pattern)
        {
            var resultData = await Task.Run(() =>
            {
                return GetRightListByPattern<T>(pattern);
            });

            return resultData;
        }

        public List<List<T>> GetRightListByPattern<T>(string pattern)
        {
            List<List<T>> result = new List<List<T>>();
            var _muxer = StackExchangeRedisManager.Instance;
            var _db = db.GetDatabase();
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: pattern, database: _db.Database);
                foreach (var keyName in keys)
                {
                    var keyValue = new List<T>();

                    keyValue.AddRange(GetListAllData<T>(keyName));

                    result.Add(keyValue);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据key的定义规则模糊查询所有符合规则的key, 以列表形式返回
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> GetKeyListByPattern(string pattern)
        {
            List<string> result = new List<string>();
            var _muxer = StackExchangeRedisManager.Instance;
            var _db = db.GetDatabase();
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: pattern, database: _db.Database);
                foreach (var keyName in keys)
                {
                    result.Add(keyName);
                }
            }

            return result;
        }

        #endregion
    }
}
