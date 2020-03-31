namespace Qoollo.Redis.Net.Configuration
{
    public class RedisServiceConfiguration : IRedisServiceConfiguration
    {
        public string RedisIp { get; set; }

        public int RedisPort { get; set; }

        public int RedisSyncOperationsTimeoutMs { get; set; }

        public string ClientName { get; set; }
    }
}