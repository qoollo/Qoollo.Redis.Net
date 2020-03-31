using StackExchange.Redis;

namespace Qoollo.Redis.Net.Configuration
{
    public class RedisConfigBuilder
    {
        public static ConfigurationOptions ParseRedisConfigurationOptions(IRedisServiceConfiguration configuration)
        {
            var configOptions = new ConfigurationOptions();

            configOptions.EndPoints.Add($"{configuration.RedisIp}:{configuration.RedisPort}");
            configOptions.ClientName = configuration.ClientName;
            configOptions.SyncTimeout = configuration.RedisSyncOperationsTimeoutMs;

            return configOptions;
        }
    }
}