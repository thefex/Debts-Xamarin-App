using System;

namespace Debts.Services.Exceptions
{
    public interface IAppLogger
    {
        void Log(string title, string logContent);
        void Log(Exception exception, string tag="exception_tag");
        void IdentifyUser(string userId);
        void ClearUserIdentity();
    }
}