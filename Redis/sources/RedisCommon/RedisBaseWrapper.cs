namespace Redis.RedisCommon
{
    /// <summary>
    /// redis base wrapper
    /// </summary>
    public class RedisBaseWrapper
    {
        protected StackExchangeRedisBase redis;

        protected RedisBaseWrapper()
        {
            redis = new StackExchangeRedisBase();
        }
    }
}
