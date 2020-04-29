using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Qoollo.Redis.Net.Configuration;
using RedisService.Exceptions;
using StackExchange.Redis;

namespace Qoollo.Redis.Net
{
    public class RedisService : IRedisService, IDisposable
    {
        private readonly ILogger<RedisService> _logger;
        private readonly IRedisServiceConfiguration _configuration;
        private ConnectionMultiplexer _redis;
        
        public RedisService(ILogger<RedisService> logger, IRedisServiceConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            Connect();
        }

        public bool IsConnected => _redis?.IsConnected ?? false;

        public void Connect()
        {
            _logger.LogInformation($"Connecting to Redis on {_configuration.RedisIp}:{_configuration.RedisPort}...");
            _redis = ConnectionMultiplexer.Connect(RedisConfigBuilder.ParseRedisConfigurationOptions(_configuration));
            if (!IsConnected)
                throw new RedisNotConnectedException("Failed to connect to Redis.");

            _logger.LogInformation($"Connected to Redis on {_configuration.RedisIp}:{_configuration.RedisPort}.");
        }

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting from Redis...");
            if (_redis != null)
            {
                _redis.GetSubscriber()?.UnsubscribeAll();
                _redis.Dispose();
                _redis = null;
                _logger.LogInformation("Disconnected from Redis.");
            }
            else
                _logger.LogInformation("Have been already disconnected.");
        }

        public async Task<bool> SetKeyAsync(string key, byte[] data)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't set key to Redis.");

            return await _redis.GetDatabase().StringSetAsync(key, data);
        }

        public async Task<bool> SetKeyWithExpirationTimeAsync(string key, byte[] data, TimeSpan expirationTime)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't set key to Redis.");

            return await _redis.GetDatabase().StringSetAsync(key, data, expirationTime);
        }

        public async Task RequestForRemovingKeyAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't delete key: there is no connection to Redis.");

            await _redis.GetDatabase().KeyDeleteAsync(key, CommandFlags.FireAndForget);
            _logger.LogInformation($"Key '{key}' was requested for deletion");
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't delete key: there is no connection to Redis.");

            var res = await _redis.GetDatabase().KeyDeleteAsync(key);
            _logger.LogInformation($"Key '{key}' " + (res ? "was deleted." : "wasn't deleted because it didn't exist."));
            return res;
        }

        public async Task<List<byte[]>> GetListAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't get list: there is no connection to Redis.");

            return (await _redis.GetDatabase().ListRangeAsync(key))
                .Select(v => (byte[]) v)
                .ToList();
        }

        public async Task<byte[]> GetValueAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't get value: there is no connection to Redis.");

            return await _redis.GetDatabase().StringGetAsync(key);
        }

        public async Task<List<byte[]>> GetValuesAsync(IEnumerable<string> keys)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't get value: there is no connection to Redis.");
            
            return (await _redis.GetDatabase().StringGetAsync(keys.Select(k => (RedisKey) k).ToArray()))
                .Select(v => (byte[]) v)
                .ToList();
        }

        public async Task<List<string>> GetKeysByPatternAsync(string pattern)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't get values: there is no connection to Redis.");

            return await Task.Run(() =>
            {
                List<string> result = null;
                try
                {
                    IServer server = _redis.GetServer(_redis.GetEndPoints().ToList().Single());
                    result = server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Error in getting keys.");
                    result = new List<string>();
                }
                return result;
            });
        }

        public async Task<long> ListLeftPushAsync(string key, byte[] data)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't add data to list: there is no connection to Redis.");

            return await _redis.GetDatabase().ListLeftPushAsync(key, data);
        }

        public async Task<long> ListRightPushAsync(string key, byte[] data)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't add data to list: there is no connection to Redis.");

            return await _redis.GetDatabase().ListRightPushAsync(key, data);
        }
        
        public async Task<byte[]> ListLeftPopAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't add data to list: there is no connection to Redis.");

            return await _redis.GetDatabase().ListLeftPopAsync(key);
        }

        public async Task<byte[]> ListRightPopAsync(string key)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't add data to list: there is no connection to Redis.");

            return await _redis.GetDatabase().ListRightPopAsync(key);
        }
        
        public async Task<byte[]> ListLeftBlockingPopAsync(string key)
        {
            var popvalue = await ListLeftPopAsync(key);
            while (popvalue == null)
            {
                Thread.Sleep(_configuration.RedisBlockingOperationsTimeoutMs);
                popvalue = await ListLeftPopAsync(key);
            }

            return popvalue;
        }

        public async Task<byte[]> ListRightBlockingPopAsync(string key)
        {
            var popvalue = await ListRightPopAsync(key);
            while (popvalue == null)
            {
                Thread.Sleep(_configuration.RedisBlockingOperationsTimeoutMs);
                popvalue = await ListRightPopAsync(key);
            }

            return popvalue;
        }

        public async Task<long> ListRemoveAsync(string key, byte[] data)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't remove the data from the list: there is no connection to Redis.");

            return await _redis.GetDatabase().ListRemoveAsync(key, data);
        }

        public void SubscribeToChannel(string channel, Action<string, byte[]> handler)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't subscribe to channel '{channel}': " +
                    "there is no connection to Redis.");

            _redis.GetSubscriber().Subscribe(channel, (ch, v) => handler(ch, v));
            _logger.LogInformation($"Subscribed to Redis channel '{channel}'.");
        }
        
        public void PublishToChannel(string channel, byte[] data)
        {
            if (!IsConnected)
                throw new RedisNotConnectedException($"Can't publish to channel '{channel}': " +
                                                     "there is no connection to Redis.");

            _redis.GetSubscriber().Publish(channel, data);
            _logger.LogInformation($"Data was successfully published to Redis channel '{channel}'.");
        }


        #region IDisposable

        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
            }
        }

        ~RedisService()
        {
            _logger.LogTrace("Destructing RedisService.");
            Dispose(false);
        }

        public void Dispose()
        {
            _logger.LogTrace("Disposing RedisService.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}