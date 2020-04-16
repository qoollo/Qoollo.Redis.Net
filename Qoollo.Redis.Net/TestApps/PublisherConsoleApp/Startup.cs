using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Qoollo.Redis.Net;

namespace PublisherConsoleApp
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
            for (int i = 0; i < 10; i++)
            {
                var message = $"Hello. This is message {i}";
                var data = Encoding.UTF8.GetBytes(message);
                _redisService.PublishToChannel(_channelName, data);
                
                Console.WriteLine($"Redis Published message: {message}");
                Console.WriteLine("Now waiting for 5 seconds...");
                Console.WriteLine();
                
                Thread.Sleep(5000);
            }
        }
    }
}