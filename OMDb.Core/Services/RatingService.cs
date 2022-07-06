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

namespace OMDb.Core.Services
{
    public class RatingService
    {
        private static IEnumerable<IRate> Rates;
        public static void Init()
        {
            var assembiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(AppContext.BaseDirectory,"Plugins"), "*.dll", System.IO.SearchOption.TopDirectoryOnly)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IRate>().Export<IRate>().Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assembiles, conventions);

            using(var container = configuration.CreateContainer())
            {
                Rates = container.GetExports<IRate>();
            }
            foreach(var rate in Rates)
            {
                var r = rate.Rate();
            }
        }
    }
}
