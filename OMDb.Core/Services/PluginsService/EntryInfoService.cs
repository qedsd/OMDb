using Newtonsoft.Json;
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

            return await Task.Run<Dictionary<string, object>>(() =>
            {
                var result = export.GetEntryInfoNet(keyword);
                return result;
            });
        }

        public static async Task<string> GetEntryInfoDescriptionNet(string keyword, string dllName)
        {
            if (keyword.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                return null;
            if (EntryInfoDescriptionExports == null)
                return null;
            var export = EntryInfoDescriptionExports.FirstOrDefault(a => a.GetType().Assembly.GetName().Name.Equals(dllName));

            return await Task.Run<string>(() =>
            {
                var result = export.GetEntryInfoDescriptionNet(keyword);
                return result;
            });
        }
    }
}
