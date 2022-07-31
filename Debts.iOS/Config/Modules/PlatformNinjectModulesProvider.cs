using System.Collections.Generic;
using Debts.Config.Modules;
using Ninject.Modules;

namespace Debts.iOS.Config.Modules
{
    public class PlatformNinjectModulesProvider : AppNinjectModulesProvider
    {
        protected override IEnumerable<INinjectModule> GetPlatformSpecificModules()
        {
            yield return new PlatformNinjectModule();
        }
    }
}