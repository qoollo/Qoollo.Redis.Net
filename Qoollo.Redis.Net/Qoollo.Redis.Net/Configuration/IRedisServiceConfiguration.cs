﻿namespace Qoollo.Redis.Net.Configuration
{
    public interface IRedisServiceConfiguration
    {
        string RedisIp { get; set; }

        int RedisPort { get; set; }

        int RedisSyncOperationsTimeoutMs { get; set; }
        
        int RedisBlockingOperationsTimeoutMs { get; set; }

        string ClientName { get; set; }
    }
}