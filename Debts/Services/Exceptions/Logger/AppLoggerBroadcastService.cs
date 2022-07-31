using System;

namespace Debts.Services.Exceptions.Logger
{
    public class AppLoggerBroadcastService : IAppLogger
    {
        private readonly IAppLogger[] _appLoggers;

        public AppLoggerBroadcastService(params IAppLogger[] appLoggers)
        {
            _appLoggers = appLoggers;
        }
        
        public void Log(string tag, string logContent)
        {
            foreach(var appLogger in _appLoggers)
                appLogger.Log(tag, logContent);
        }

        public void Log(Exception exception, string tag = "exception_tag")
        {
            foreach(var appLogger in _appLoggers)
                appLogger.Log(exception, tag);
        }

        public void IdentifyUser(string userId)
        {
            foreach(var appLogger in _appLoggers)
                appLogger.IdentifyUser(userId);
        }

        public void ClearUserIdentity()
        {
            foreach(var appLogger in _appLoggers)
                appLogger.ClearUserIdentity();
        }
    }
}