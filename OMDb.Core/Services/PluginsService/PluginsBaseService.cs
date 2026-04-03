using OMDb.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services.PluginsService
{
    public class PluginsBaseService
    {
        public static IEnumerable<IRate> Rates;
        public static IEnumerable<IEntryInfo> EntryInfoExports;
        public static IEnumerable<IEntryInfoDescription> EntryInfoDescriptionExports;
        public static void Init()
        {
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "Plugins")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppContext.BaseDirectory, "Plugins"));
                return;
            }
            var assembiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins"), "*.dll", System.IO.SearchOption.TopDirectoryOnly)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IRate>().Export<IRate>().Shared();
            conventions.ForTypesDerivedFrom<IEntryInfo>().Export<IEntryInfo>().Shared();
            conventions.ForTypesDerivedFrom<IEntryInfoDescription>().Export<IEntryInfoDescription>().Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assembiles, conventions);

            using (var container = configuration.CreateContainer())
            {
                Rates = container.GetExports<IRate>();
                EntryInfoExports = container.GetExports<IEntryInfo>();
                EntryInfoDescriptionExports= container.GetExports<IEntryInfoDescription>();
            }
        }
    }
}
