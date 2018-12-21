using Jiajue.BeiJi.Redis.RedisCommon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jiajue.BeiJi.Redis.RedisWrapper
{
    /// <summary>
    /// Redis Sorted Set
    /// </summary>
    [Obsolete("暂时没用,待需要使用时注释此Attribute")]
    public sealed class RedisSortedSetWrapper : RedisBaseWrapper
    {
        public RedisSortedSetWrapper()
            : base()
        {
        }

        #region 同步执行

        /// <summary>
        /// 无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T val, double score)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SortedSetAdd(key, redis.ConvertJson<T>(val), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Remove<T>(string key, T val)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SortedSetRemove(key, redis.ConvertJson<T>(val)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetRange<T>(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db =>
            {
                var val = db.SortedSetRangeByRank(key);
                return redis.ConvertList<T>(val);
            });
        }

        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLength(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SortedSetLength(key));

        }
        #endregion

        #region 异步执行

        /// <summary>
        /// 异步无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync<T>(string key, T val, double score)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SortedSetAddAsync(key, redis.ConvertJson<T>(val), score));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync<T>(string key, T val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SortedSetRemoveAsync(key, redis.ConvertJson<T>(val)));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<T>> GetRangeAsync<T>(string key)
        {
            key = redis.AddKey(key);
            var val = await redis.DoSave(db => db.SortedSetRangeByRankAsync(key));
            return redis.ConvertList<T>(val);
        }

        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> GetLengthAsync(string key)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SortedSetLengthAsync(key));

        }
        #endregion
    }
}
