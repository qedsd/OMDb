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
    public class EntryInfoService: PluginsBaseService
    {
        public static Task<Dictionary<string,object>> GetEntryInfo(string keyword, string dllName)
        {
            if (Rates != null)
            {
                foreach (var entryInfo in EntryInfos)
                {
                    if (entryInfo.GetType().Assembly.GetName().Name.Equals(dllName))
                    {
                        return (Task.Run<Dictionary<string, object>>(()=> entryInfo.EntryInfo(keyword)));
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
