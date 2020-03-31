using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qoollo.Redis.Net.Configuration;

namespace Qoollo.Redis.Net
{
    public static class RedisServiceProviderExtensions
    {
        public static void AddRedisService(this IServiceCollection services, IConfigurationSection configuration)
        {
            var config = configuration.Get<RedisServiceConfiguration>();
            services.AddTransient<IRedisServiceConfiguration>(serviceProvider => config);

            services.AddSingleton<IRedisService, RedisService>();
        }
    }
}