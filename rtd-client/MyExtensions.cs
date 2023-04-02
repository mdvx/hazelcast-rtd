using Hazelcast;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;


namespace HazelcastRTD
{
    internal static class MyExtensions
    {
        public static HazelcastOptionsBuilder WithConsoleLogger(this HazelcastOptionsBuilder builder, LogLevel hazelcastLogLevel = LogLevel.None)
        {
            return builder
                .With("Logging:LogLevel:Default", "None")
                .With("Logging:LogLevel:System", "Information")
                .With("Logging:LogLevel:Microsoft", "Information")
                .With("Logging:LogLevel:Hazelcast", hazelcastLogLevel.ToString())
                .With((configuration, options) =>
                {
                    //configure logging factory and add the console provider
                    //options.LoggerFactory.Creator = () => LoggerFactory.Create(loggingBuilder =>
                    //    loggingBuilder
                    //        .AddConfiguration(configuration.GetSection("logging"))
                    //        .AddConsole());
                });
        }
    }
}
