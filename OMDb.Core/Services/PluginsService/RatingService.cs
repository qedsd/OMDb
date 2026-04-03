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
    public class RatingService: PluginsBaseService
    {
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

        public static Models.Rating GetRatings(string id, string dllName)
        {
            if (Rates != null)
            {
                foreach (var rate in Rates)
                {
                    if (rate.GetType().Assembly.GetName().Name.Equals(dllName))
                    {
                        return rate.Rate(id);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

    }
}
