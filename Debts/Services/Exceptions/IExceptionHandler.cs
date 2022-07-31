using System;

namespace Debts.Services.Exceptions
{
    public interface IExceptionHandler
    {
        bool HandleException(Exception e);
    }
}