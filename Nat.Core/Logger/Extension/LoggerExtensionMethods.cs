using System;
using System.Net.Http;

namespace Nat.Core.Logger.Extension
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Creates a logging scope for Functions
        /// </summary>
        /// <typeparam name="TState">Type of log</typeparam>
        /// <param name="logger">Extended type object</param>
        /// <param name="state">Object to be logged</param>
        /// <returns></returns>
        public static IDisposable BeginFunctionScope<TState>(this NatLogger logger, TState state)
        {
            return logger.BeginScope<TState>(state, LogContext.Function);
        }

        /// <summary>
        /// Creates a logging scope for Services
        /// </summary>
        /// <typeparam name="TState">Type of log</typeparam>
        /// <param name="logger">Extended type object</param>
        /// <param name="state">Object to be logged</param>
        /// <returns></returns>
        public static IDisposable BeginServiceScope<TState>(this NatLogger logger, TState state)
        {
            return logger.BeginScope<TState>(state, LogContext.Service);
        }

        /// <summary>
        /// Logs string message with log level "Information"
        /// </summary>
        /// <param name="logger">Extended type object</param>
        /// <param name="message">Message to be logged</param>
        public static void LogInformation(this NatLogger logger, string message)
        {
            string formatter(string state, System.Exception exception)
            {
                return state;
            }
            //logger.Log<string>(LogLevel.Information, null, message, null, formatter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void LogError(this NatLogger logger, System.Exception ex)
        {
            string formatter(string state, System.Exception exception)
            {
                return exception.Message;
            }
            logger.Log<string>(LogLevel.Error, null, null, ex, formatter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="req"></param>
        public static void LogRequest(this NatLogger logger, HttpRequestMessage req)
        {
            //string formatter(HttpRequestMessage state, Exception ex)
            //{
            //    return JsonConvert.SerializeObject(state.Content, new JsonSerializerSettings()
            //    {
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //        Formatting = Formatting.Indented
            //    });
            //}
            //logger.Log(LogLevel.Information, null, req, null, formatter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="res"></param>
        public static void LogResponse(this NatLogger logger, HttpResponseMessage res)
        {
            //string formatter(HttpResponseMessage state, Exception ex)
            //{
            //    return JsonConvert.SerializeObject(state.Content, new JsonSerializerSettings()
            //    {
            //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //        Formatting = Formatting.Indented
            //    });
            //}
            //logger.Log(LogLevel.Information, null, res, null, formatter);
        }
    }
}
