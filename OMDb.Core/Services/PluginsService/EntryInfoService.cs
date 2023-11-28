using OMDb.Core.Interfaces;
using OMDb.Core.Utils.Extensions;
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
    public class EntryInfoService : PluginsBaseService
    {
        public static async Task<Dictionary<string, object>> GetEntryInfoNet(string keyword, string dllName)
        {
            if (keyword.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                return null;
            if (EntryInfoExports == null)
                return null;
            var export = EntryInfoExports.FirstOrDefault(a => a.GetType().Assembly.GetName().Name.Equals(dllName));
            return await Task.Run<Dictionary<string, object>>(() => export.GetEntryInfoNet(keyword));
        }

    }
}
