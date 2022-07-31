using System;
using MvvmCross;

namespace Debts.Services.Exceptions
{
    [Preserve]
    public class ExceptionHandlerAttribute : Attribute
    {
        public Type ExceptionTypeToCatch { get; private set; }

        public Type ExceptionHandlerType { get; private set; }

        public ExceptionHandlerAttribute(Type exceptionHandlerType, Type exceptionTypeToCatch)
        {
            ExceptionHandlerType = exceptionHandlerType;
            ExceptionTypeToCatch = exceptionTypeToCatch;
        }
    }
}