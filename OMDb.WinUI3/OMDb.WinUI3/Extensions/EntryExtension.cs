using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Extensions
{
    public static class EntryExtension
    {
        public static List<Core.Models.ExtractsLineBase> GetExtractsLines(this Core.Models.Entry entry)
        {
            var metadata = entry.GetMetadata();
            if(metadata != null)
            {
                return metadata.ExtractsLines?.ToList();
            }
            else
            {
                return null;
            }
        }
        public static Core.Models.EntryMetadata GetMetadata(this Core.Models.Entry entry)
        {
            var fullPath = entry.GetFullPath();
            if (!string.IsNullOrEmpty(fullPath))
            {
                return Core.Models.EntryMetadata.Read(Path.Combine(fullPath, Services.ConfigService.MetadataFileNmae));
            }
            else
            {
                return null;
            }
        }
        public static string GetFullPath(this Core.Models.Entry entry)
        {
            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (s != null && !string.IsNullOrEmpty(entry.Path))
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(s.StoragePath), entry.Path);
            }
            else
            {
                return null;
            }
        }
    }
}
