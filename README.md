# Qoollo.Redis.Net

Qoollo.Redis.Net is a convinient wrapper over StackExchange.Redis.

## Installation
Qoollo.Redis.Net can be installed via the nuget UI (as Qoollo.Redis.Net), or via the nuget package manager console:
```
PM> Install-Package Qoollo.Redis.Net
```
After that you need to specify Redis configuration section in appsettings.json
```
...
"Redis": {
        "RedisIp": "<some_ip>",
        "RedisPort": 6379,
        "RedisSyncOperationsTimeoutMs": 5000,
        "ClientName": "<some_client_name>"
    },
...
```
Finally you can simply registry RedisService
```
...
services.AddRedisService(Configuration.GetSection("Redis"));
...
```

## Usage

This wrapper provides IRedisService interface via which one you can inject RedisService dependecy in your class, such as:
```
public class MyClass
{
    private readonly IRedisService _redisService;
  
    public MyClass(IRedisService redisService)
    {
        _redisService = redisService;
    }
}
```
IRedisService provides the following methods to use:
```
public interface IRedisService
    {
        /// <summary>True if connection to Redis established, false if not.</summary>
        bool IsConnected { get; }

        /// <summary>Connect to Redis.</summary>
        void Connect();

        /// <summary>Disconnect from Redis.</summary>
        void Disconnect();

        /// <summary>Subscribe handler to channel.</summary>
        /// <param name="channel">Channel's name.</param>
        /// <param name="handler">
        /// Handler for event of receiving message from channel. 
        /// First arg is channel's name (string), second is received message from redis (byte[]).
        /// </param>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        void SubscribeToChannel(string channel, Action<string, byte[]> handler);

        /// <summary>Adds key with given data to Redis.</summary>
        /// <returns>True if key has been already existed and was overwritten, false if not.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<bool> SetKeyAsync(string key, byte[] data);

        /// <summary>
        /// Adds key with given data to Redis, that will be expired (automatically removed) after given expiratin time.
        /// </summary>
        /// <returns>True if key has been already existed and was overwritten, false if not.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<bool> SetKeyWithExpirationTimeAsync(string key, byte[] data, TimeSpan expirationTime);

        /// <summary>Removes the specified key. A key is ignored if it does not exist.</summary>
        /// <returns>True if the key was removed.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<bool> DeleteKeyAsync(string key);

        /// <summary>
        /// Sends request for removing the specified key (fire and forget). A key is ignored if it does not exist.
        /// </summary>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task RequestForRemovingKeyAsync(string key);

        /// <summary>Get value from Redis by it's key.</summary>
        /// <returns>Value or null if key doesn't exist.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<byte[]> GetValueAsync(string key);

        /// <summary>Get values from Redis by theirs' keys.</summary>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<List<byte[]>> GetValuesAsync(IEnumerable<string> keys);

        /// <summary>Get keys from Redis which match given pattern.</summary>
        /// <param name="pattern">Pattern for keys.</param>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<List<string>> GetKeysByPatternAsync(string pattern);

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does
        /// not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <returns>The length of the list after the push operations.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<long> ListLeftPushAsync(string key, byte[] data);

        /// <summary>
        /// Insert the specified value at the end of the list stored at key. If key does
        /// not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <returns>The length of the list after the push operations.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<long> ListRightPushAsync(string key, byte[] data);

        /// <summary>
        /// Removes specified value from the list stored under key.
        /// </summary>
        /// <returns>The length of the list after the remove operations.</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<long> ListRemoveAsync(string key, byte[] data);

        /// <summary>Get all elements from list in Redis by given key.</summary>
        /// <param name="key"></param>
        /// <returns>List with all elements from list in Redis</returns>
        /// <exception cref="RedisNotConnectedException">Raised when there is no connection to Redis.</exception>
        Task<List<byte[]>> GetListAsync(string key);
    }
```
