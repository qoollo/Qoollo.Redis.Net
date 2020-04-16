using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Qoollo.Redis.Net;

namespace PublisherConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = GetConfiguration("appconfig.json");
            var serviceProvider = ConfigureServiceProvider(configuration);
            var app = serviceProvider.GetService<Startup>();
            app.Run();
        }

        private static IConfiguration GetConfiguration(string fileName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(fileName);
 
            return new ConfigurationBuilder()
                .AddJsonFile(fileName, true, true)
                .Build();
        }
        
        private static IServiceProvider ConfigureServiceProvider(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRedisService(configuration.GetSection("Redis"));
            serviceCollection.AddTransient<Startup>()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddNLog("nlog.config");
                });
            
            return serviceCollection.BuildServiceProvider();
        }
    }
}