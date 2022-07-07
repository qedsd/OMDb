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
            if(!System.IO.Directory.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "Plugins")))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(AppContext.BaseDirectory, "Plugins"));
                return;
            }
            var assembiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(AppContext.BaseDirectory,"Plugins"), "*.dll", System.IO.SearchOption.TopDirectoryOnly)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IRate>().Export<IRate>().Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assembiles, conventions);

            using(var container = configuration.CreateContainer())
            {
                Rates = container.GetExports<IRate>();
            }
        }
        public static IEnumerable<Models.Rating> GetRatings(string id)
        {
            if (Rates != null)
            {
                List<Models.Rating> ratings = new List<Models.Rating>();
                foreach (var rate in Rates)
                {
                    ratings.Add(rate.Rate(id));
                }
                return ratings;
            }
            else
            {
                return null;
            }
        }
    }
}
