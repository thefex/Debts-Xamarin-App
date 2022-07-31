using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MvvmCross;
using MvvmCross.Plugin.Messenger;

namespace Debts.Services.Exceptions
{
    [Preserve]
    class AttributeBasedExceptionHandlerFactory : IExceptionHandler
    {
        private readonly Assembly[] _assemblyToLookupIn;
        private readonly Dictionary<Type, HashSet<IExceptionHandler>> _exceptionTypeToHandlerMap;
        private bool isInitialized;

        public AttributeBasedExceptionHandlerFactory(params Assembly[] assemblyToLookupIn)
        {
            _assemblyToLookupIn = assemblyToLookupIn;
            _exceptionTypeToHandlerMap = new Dictionary<Type, HashSet<IExceptionHandler>>();
        }

        public bool HandleException(Exception e)
        {
            lock (this)
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    InitializeExceptionHandlers();
                }
            }

            var exceptionToHandleType = e.GetType();
            if (!_exceptionTypeToHandlerMap.ContainsKey(exceptionToHandleType))
                return false;

            foreach (var exceptionHandler in _exceptionTypeToHandlerMap[exceptionToHandleType])
                exceptionHandler.HandleException(e);

            return true;
        }

        private void InitializeExceptionHandlers()
        {
            var anonymousExceptionHandlers = _assemblyToLookupIn.SelectMany(x => x.DefinedTypes)
                .Where(x => x.IsDefined(typeof(ExceptionHandlerAttribute)))
                .Select(x =>
                {
                    var exceptionHandlerAttribute = x.GetCustomAttribute<ExceptionHandlerAttribute>();
                    return new
                    {
                        HandledExceptionType = exceptionHandlerAttribute.ExceptionTypeToCatch,
                        ExceptionHandler =
                            Activator.CreateInstance(exceptionHandlerAttribute.ExceptionHandlerType) as IExceptionHandler
                    };
                }).ToList();

            foreach (var anonymousExceptionHandlerInfo in anonymousExceptionHandlers)
            {
                if (!_exceptionTypeToHandlerMap.ContainsKey(anonymousExceptionHandlerInfo.HandledExceptionType))
                    _exceptionTypeToHandlerMap.Add(anonymousExceptionHandlerInfo.HandledExceptionType,
                        new HashSet<IExceptionHandler>());

                _exceptionTypeToHandlerMap[anonymousExceptionHandlerInfo.HandledExceptionType].Add(
                    anonymousExceptionHandlerInfo.ExceptionHandler);
            }
        }
    }
}