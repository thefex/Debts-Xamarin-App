using System.Collections.Generic;
using System.Linq;
using Ninject.Modules;

namespace Debts.Config.Modules
{
    public abstract class AppNinjectModulesProvider
    {
        protected abstract IEnumerable<INinjectModule> GetPlatformSpecificModules();

        public virtual INinjectModule[] GetNinjectModules()
        {
            IList<INinjectModule> ninjectModules = new List<INinjectModule>()
            {
                new PortableMainNinjectModule(),
                new PortableServicesNinjectModule()
            };

            foreach (var platformSpecificModule in GetPlatformSpecificModules())
                ninjectModules.Add(platformSpecificModule);

            return ninjectModules.ToArray();
        }
    }
}