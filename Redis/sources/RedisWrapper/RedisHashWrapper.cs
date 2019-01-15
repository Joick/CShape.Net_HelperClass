using Redis.RedisCommon;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.RedisWrapper
{
    /// <summary>
    /// Redis Hash
    /// </summary>
    public sealed class RedisHashWrapper : RedisBaseWrapper
    {
        public RedisHashWrapper()
            : base()
        {
        }

        #region 同步执行

        /// <summary>
        /// 是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool Exists(string key, string dataKey)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.HashExists(key, dataKey));
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Set<T>(string key, string dataKey, T val)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                string json = redis.ConvertJson(val);
                return db.HashSet(key, dataKey, json);
            });
        }

        /// <summary>
        /// 从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool Delete(string key, string dataKey)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.HashDelete(key, dataKey));
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public long Remove(string key, List<RedisValue> dataKey)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.HashDelete(key, dataKey.ToArray()));
        }

        /// <summary>
        /// 从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T Get<T>(string key, string dataKey)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.HashGet(key, dataKey);

                if (!val.HasValue)
                    return default(T);

                return redis.ConvertObj<T>(val);
            });
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double DoIncrement(string key, string dataKey, double val = 1)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.HashIncrement(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeyKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public double DoDecrement(string key, string dataKey, double val = 1)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.HashDecrement(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetAllKeys<T>(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.HashKeys(key);
                return redis.ConvertList<T>(val);
            });
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> GetAllNodeKey(string key)
        {
            List<string> keyChild = new List<string>();
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var vals = db.HashKeys(key);

                foreach (var item in vals)
                {
                    keyChild.Add(item.ToString());
                }

                return keyChild;
            });
        }

        /// <summary>
        /// 从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public List<T> GetAllData<T>(string key)
        {
            List<T> results = new List<T>();
            key = redis.AddKey(key);
            return redis.DoSave<List<T>>(db =>
            {
                var val = db.HashGetAll(key);

                foreach (var item in val)
                {
                    if (item.Value.HasValue)
                        results.Add(redis.ConvertObj<T>(item.Value));
                }

                return results;
            });
        }

        #endregion

        #region 异步执行

        /// <summary>
        /// 异步是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key, string dataKey)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.HashExistsAsync(key, dataKey));
        }

        /// <summary>
        /// 异步存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, string dataKey, T val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db =>
            {
                string json = redis.ConvertJson(val);
                return db.HashSetAsync(key, dataKey, json);
            });
        }

        /// <summary>
        /// 异步从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key, string dataKey)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.HashDeleteAsync(key, dataKey));
        }

        /// <summary>
        /// 异步移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<long> RemoveAsync(string key, List<RedisValue> dataKey)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.HashDeleteAsync(key, dataKey.ToArray()));
        }

        /// <summary>
        /// 从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, string dataKey)
        {
            key = redis.AddKey(key);
            string val = await redis.DoSave(db => db.HashGetAsync(key, dataKey));
            return redis.ConvertObj<T>(val);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<double> DoIncrementAsync(string key, string dataKey, double val = 1)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.HashIncrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeyKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<double> DoDecrementAsync(string key, string dataKey, double val = 1)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.HashDecrementAsync(key, dataKey, val));
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllKeysAsync<T>(string key)
        {
            key = redis.AddKey(key);
            var val = await redis.DoSave(db => db.HashKeysAsync(key));
            return redis.ConvertList<T>(val);
        }
        #endregion
    }
}
