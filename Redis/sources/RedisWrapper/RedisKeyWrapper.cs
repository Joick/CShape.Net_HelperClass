using Redis.RedisCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.RedisWrapper
{
    /// <summary>
    /// Redis Key
    /// </summary>
    public sealed class RedisKeyWrapper : RedisBaseWrapper
    {
        public RedisKeyWrapper()
            : base()
        {
        }


        #region 同步执行

        /// <summary>
        /// 删除单个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.KeyDelete(key));
        }
        /// <summary>
        /// 删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long Delete(List<string> key)
        {
            List<string> Keys = key.Select(redis.AddKey).ToList();
            return redis.DoSave(db => db.KeyDelete(redis.ConvertRedisKeys(Keys)));
        }

        /// <summary>
        /// 重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public bool Rename(string key, string newKey)
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.KeyRename(key, newKey));
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public bool Expire(string key, TimeSpan? exp = default(TimeSpan?))
        {
            key = redis.AddKey(key);
            return redis.DoSave(db => db.KeyExpire(key, exp));
        }

        /// <summary>
        /// 根据key的定义规则模糊查询所有符合规则的key, 以列表形式返回
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> GetListByPattern(string pattern)
        {
            return redis.GetKeyListByPattern(pattern);
        }

        #endregion

        #region 异步执行

        /// <summary>
        /// 异步删除单个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string key)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.KeyDeleteAsync(key));
        }

        /// <summary>
        /// 异步删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> DeleteAsync(List<string> key)
        {
            List<string> Keys = key.Select(redis.AddKey).ToList();
            return await redis.DoSave(db => db.KeyDeleteAsync(redis.ConvertRedisKeys(Keys)));
        }

        /// <summary>
        ///  异步重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public async Task<bool> RenameAsync(string key, string newKey)
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.KeyRenameAsync(key, newKey));
        }

        /// <summary>
        /// 异步设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public async Task<bool> ExpireAsync(string key, TimeSpan? exp = default(TimeSpan?))
        {
            key = redis.AddKey(key);
            return await redis.DoSave(db => db.KeyExpireAsync(key, exp));
        }

        #endregion
    }
}
