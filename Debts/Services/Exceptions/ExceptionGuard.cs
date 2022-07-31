using System;
using Microsoft.AppCenter.Crashes;
using MvvmCross;

namespace Debts.Services.Exceptions
{
    [Preserve]
    public class ExceptionGuard
    {
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IAppLogger _appLogger;

        public ExceptionGuard(IExceptionHandler exceptionHandler, IAppLogger appLogger)
        {
            _exceptionHandler = exceptionHandler;
            _appLogger = appLogger;
        }

        public void OnException(Exception e)
        {
            if (!_exceptionHandler.HandleException(e))
                LogException(e);
        }

        private void LogException(Exception e)
        {
            _appLogger.Log(e);
            Crashes.TrackError(e);
        }
    }
}