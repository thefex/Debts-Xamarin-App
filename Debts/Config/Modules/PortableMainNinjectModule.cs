using Debts.Messenging;
using Debts.Services.Exceptions;
using Debts.Services.Exceptions.Logger;
using Ninject.Modules;

namespace Debts.Config.Modules
{
    public class PortableMainNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ExceptionGuard>()
                .ToMethod(ctx => new ExceptionGuard(new AttributeBasedExceptionHandlerFactory(), new AppLoggerBroadcastService(new ConsoleLogger())))
                .InSingletonScope();

            Bind<MessageQueue>()
                .ToSelf()
                .InSingletonScope();
        }
    }
}