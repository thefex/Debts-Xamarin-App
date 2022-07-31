using System;
using System.Collections;
using System.Text;

namespace Debts.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetExceptionDetailedMessage(this Exception exception)
        {
            StringBuilder exceptionStringBuilder = new StringBuilder();
            exception.WriteExceptionDetailedMessage(exceptionStringBuilder);
            return exceptionStringBuilder.ToString();
        }
        
        private static void WriteExceptionDetailedMessage(this Exception exception, StringBuilder builderToFill, int level = 1)
        {
            var indent = new string(' ', level);
            builderToFill.AppendLine(indent + "=== INNER EXCEPTION ===");                

            Action<string> append = (prop) =>
            {
                var propInfo = exception.GetType().GetProperty(prop);
                object val = propInfo?.GetValue(exception);

                if (val != null)
                {
                    builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, val.ToString(), Environment.NewLine);
                }
            };

            append("Message");
            append("HResult");
            append("HelpLink");
            append("Source");
            append("StackTrace");
            append("TargetSite");

            foreach (DictionaryEntry de in exception.Data)
            {
                builderToFill.AppendFormat("{0} {1} = {2}{3}", indent, de.Key, de.Value, Environment.NewLine);
            }

            if (exception.InnerException != null)
            {
                WriteExceptionDetailedMessage(exception.InnerException, builderToFill, ++level);
            }
        }
    }
}