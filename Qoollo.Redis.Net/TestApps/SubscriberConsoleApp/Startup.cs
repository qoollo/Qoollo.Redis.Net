using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Qoollo.Redis.Net;

namespace SubscriberConsoleApp
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly IRedisService _redisService;
        private const string _channelName = "foo";

        public Startup(ILogger<Startup> logger, IRedisService redisService)
        {
            _logger = logger;
            _redisService = redisService;
        }
        
        public void Run()
        {
            _redisService.SubscribeToChannel(_channelName, (channel, data) =>
            {
                var message = Encoding.UTF8.GetString(data, 0, data.Length);
                Console.WriteLine($"New message recieved: {message}");
                Console.WriteLine();
            });
            
            Console.WriteLine($"Successfully subscribed to channel {_channelName}. Pending new incoming messages...");
            WaiteForIncomingMessages();
        }

        private void WaiteForIncomingMessages()
        {
            while (true)
                ;
        }
    }
}