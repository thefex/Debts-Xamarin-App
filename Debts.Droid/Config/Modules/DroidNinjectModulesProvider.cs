using System.Collections.Generic;
using Debts.Config.Modules;
using Ninject.Modules;

namespace Debts.Droid.Config.Modules
{
    public class DroidNinjectModulesProvider : AppNinjectModulesProvider
    {
        protected override IEnumerable<INinjectModule> GetPlatformSpecificModules()
        {
            yield return new PlatformNinjectModule();
        }
    }
}