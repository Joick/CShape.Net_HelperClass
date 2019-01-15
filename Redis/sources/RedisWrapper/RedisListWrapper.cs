using Redis.RedisCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.RedisWrapper
{
    /// <summary>
    /// Redis List
    /// </summary>
    public sealed class RedisListWrapper : RedisBaseWrapper
    {
        public RedisListWrapper()
            : base()
        {
        }

        #region 同步执行

        /// <summary>
        /// 移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Remove<T>(string key, T val)
        {
            key = redis.AddKey(key);
            redis.DoSave(db => db.ListRemove(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Remove(string key)
        {
            key = redis.AddKey(key);
            redis.DoSave(db => db.KeyDelete(key));
        }

        /// <summary>
        /// 移除前缀相同的List
        /// </summary>
        /// <param name="pattern">写法:*xxxxx*  *表示通配符</param>
        public void RemoveByPattern(string pattern)
        {
            redis.RemoveByPattern(pattern);
        }

        /// <summary>
        /// 获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetRange<T>(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.ListRange(key);
                return redis.ConvertList<T>(val);
            });
        }

        /// <summary>
        /// 插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void PushToQueue<T>(string key, T val)
        {
            key = redis.AddKey(key);
            redis.DoSave(db => db.ListRightPush(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T PopfromQueue<T>(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.ListRightPop(key);
                if (val.HasValue)
                    return redis.ConvertObj<T>(val);
                else
                    return default(T);

            });
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void PushToStack<T>(string key, T val)
        {
            key = redis.AddKey(key);
            redis.DoSave(db => db.ListLeftPush(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T PopFromStack<T>(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.ListLeftPop(key);
                return redis.ConvertObj<T>(val);
            });
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLength(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.ListLength(key));
        }

        /// <summary>
        /// 根据key，获取List缓存的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetAllData<T>(string key)
        {
            return redis.GetListAllData<T>(key);
        }

        /// <summary>
        /// 根据字符串模糊查询所有符合的key的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<List<T>> GetListPattern<T>(string key)
        {
            return redis.GetRightListByPattern<T>(key);
        }

        #endregion

        #region 异步执行

        /// <summary>
        /// 移除List内部指定的值（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public async Task<long> RemoveAsync<T>(string key, T val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.ListRemoveAsync(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 获取指定Key的List（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> RangeAsync<T>(string key)
        {
            key = redis.AddKey(key);
            var val = await redis.DoSave(db => db.ListRangeAsync(key));
            return redis.ConvertList<T>(val);
        }

        /// <summary>
        /// 入队（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public async Task<long> PushToQueueAsync<T>(string key, T val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.ListRightPushAsync(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 出队（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> PopFromQueueAsync<T>(string key)
        {
            key = redis.AddKey(key);
            var val = await redis.DoSave(db => db.ListRightPopAsync(key));
            return redis.ConvertObj<T>(val);
        }

        /// <summary>
        /// 入栈（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public async Task<long> PushToStackAsync<T>(string key, T val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.ListLeftPushAsync(key, redis.ConvertJson(val)));
        }

        /// <summary>
        /// 出栈（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> PopFromStackAsync<T>(string key)
        {
            key = redis.AddKey(key);
            var val = await redis.DoSave(db => db.ListLeftPopAsync(key));
            return redis.ConvertObj<T>(val);
        }

        /// <summary>
        /// 获取集合中的数量（异步）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> GetLengthAsync(string key)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.ListLengthAsync(key));
        }

        /// <summary>
        /// 获取所有数据（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllDataAsync<T>(string key)
        {
            return await redis.GetListAllDataAsync<T>(key);
        }

        /// <summary>
        /// 根据字符串模糊查询所有符合的key的值（异步）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<List<T>>> GetListPatternAsync<T>(string key)
        {
            return await redis.GetRightListPattern<T>(key);
        }

        #endregion
    }
}
