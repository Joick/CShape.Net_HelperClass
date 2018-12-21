using Jiajue.BeiJi.Redis.RedisCommon;
using System;
using System.Threading.Tasks;

namespace Jiajue.BeiJi.Redis.RedisWrapper
{
    /// <summary>
    /// Redis Set
    /// </summary>
    [Obsolete("暂时没用,待需要使用时注释此Attribute")]
    public sealed class RedisSetWrapper : RedisBaseWrapper
    {
        public RedisSetWrapper()
            : base()
        {
        }

        #region 同步执行

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Set(string key, string val)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SetAdd(key, val));
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long GetLength(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SetLength(key));
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Exists(string key, string val)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SetContains(key, val));
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Remove(string key, string val)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.SetRemove(key, val));
        }
        #endregion

        #region 异步执行

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, string val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SetAddAsync(key, val));
        }

        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> GetLengthAsync(string key)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SetLengthAsync(key));
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key, string val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SetContainsAsync(key, val));
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key, string val)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.SetRemoveAsync(key, val));
        }
        #endregion
    }
}
