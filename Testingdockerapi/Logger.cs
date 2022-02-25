using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Testingdockerapi
{
    public static class Logger <T>
    {
        public static ILoggerFactory LoggerFactory1 { get; } = new LoggerFactory();
        public static ILogger log = null;
        static Logger()
        {
            log = CreateLogger<T>();
        }

        public static ILogger CreateLogger<T>()
        {
            var logger = LoggerFactory1.CreateLogger<T>();
            return logger;
        }

    }
}
