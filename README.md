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

This wrapper provides `IRedisService` interface via which one you can inject `RedisService` dependecy in your class, such as:
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
`IRedisService` provides the following methods to use:

### IsConnected
*Property signature:* `bool IsConnected { get; }`  
*Description:* True if connection to Redis established, false if not.   

### Connect
*Method signature:* `void Connect();`  
*Description:* Connect to Redis.   

### Disconnect
*Method signature:* `void Disconnect();`  
*Description:* Disconnect from Redis.  

### SubscribeToChannel
*Method signature:* `void SubscribeToChannel(string channel, Action<string, byte[]> handler);`  
*Description:* Disconnect from Redis.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Parameters:*  
* `channel` - Channel's name.
* `handler` - Handler for event of receiving message from channel. First arg is channel's name (string), second is received message from redis (byte[]).

### SetKeyAsync
*Method signature:* `Task<bool> SetKeyAsync(string key, byte[] data);`  
*Description:* Adds key with given data to Redis.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  
*Returns:* True if key has been already existed and was overwritten, false if not.

### SetKeyWithExpirationTimeAsync
*Method signature:* `Task<bool> SetKeyWithExpirationTimeAsync(string key, byte[] data, TimeSpan expirationTime);`  
*Description:* Adds key with given data to Redis, that will be expired (automatically removed) after given expiratin time.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  
*Returns:* True if key has been already existed and was overwritten, false if not.

### DeleteKeyAsync
*Method signature:* `Task<bool> DeleteKeyAsync(string key);`  
*Description:* Removes the specified key. A key is ignored if it does not exist.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  
*Returns:* True if the key was removed.

### RequestForRemovingKeyAsync
*Method signature:* `Task RequestForRemovingKeyAsync(string key);`  
*Description:* Sends request for removing the specified key (fire and forget). A key is ignored if it does not exist.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  

### GetValueAsync
*Method signature:* `Task<byte[]> GetValueAsync(string key);`  
*Description:* Removes the specified key. A key is ignored if it does not exist.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  
*Returns:* Value or null if key doesn't exist.

### GetValuesAsync
*Method signature:* `Task<List<byte[]>> GetValuesAsync(IEnumerable<string> keys);`  
*Description:* Get values from Redis by theirs' keys.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis..  
*Returns:* List of results, where each result could be value or null if key doesn't exist.

### GetKeysByPatternAsync
*Method signature:* `Task<List<string>> GetKeysByPatternAsync(string pattern);`  
*Description:* Get keys from Redis which match given pattern.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Parameters:* `pattern` - Pattern for keys.  

### ListLeftPushAsync
*Method signature:* `Task<long> ListLeftPushAsync(string key, byte[] data);`  
*Description:* Insert the specified value at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Returns:* The length of the list after the push operations.

### ListRightPushAsync
*Method signature:* `Task<long> ListRightPushAsync(string key, byte[] data);`  
*Description:* Insert the specified value at the end of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Returns:* The length of the list after the push operations.

### ListRemoveAsync
*Method signature:* `Task<long> ListRemoveAsync(string key, byte[] data);`  
*Description:* Insert the specified value at the end of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Returns:* The length of the list after the remove operations.

### GetListAsync
*Method signature:* `Task<List<byte[]>> GetListAsync(string key);`  
*Description:* Get all elements from list in Redis by given key.  
*Exception:* `RedisNotConnectedException` - Raised when there is no connection to Redis.  
*Parameters:* `key` - Specified key.
*Returns:* List with all elements from list in Redis
