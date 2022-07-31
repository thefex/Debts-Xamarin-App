using System;
using Debts.Extensions;

namespace Debts.Services.Exceptions.Logger
{
    public class ConsoleLogger : IAppLogger
    {
        private string currentUserId = string.Empty;
        
        public void Log(string title, string logContent)
        {
            System.Diagnostics.Debug.WriteLine($"User: " + GetUserIdString() + Environment.NewLine,
                $"!!!! App Logger !!!\n" + 
                title + Environment.NewLine +
                logContent);
        }

        public void Log(Exception exception, string tag = "exception_tag")
        {
            System.Diagnostics.Debug.WriteLine(exception.GetExceptionDetailedMessage());
        }

        public void IdentifyUser(string userId)
        {
            currentUserId = userId;
        }

        public void ClearUserIdentity()
        {
            currentUserId = string.Empty;
        }

        private string GetUserIdString()
        {
            return string.IsNullOrEmpty(currentUserId) ? "not identified" : currentUserId;
        }
    }
}