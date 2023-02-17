using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 摘抄台词
    /// </summary>
    public class ExtractsLine: ExtractsLineBase
    {
        public string EntryName { get; set; }
        public string EntryId { get; set; }
        public string DbId { get; set; }
        public static ExtractsLine Create(ExtractsLineBase extractsLineBase, Entry entry)
        {
            if(extractsLineBase == null || entry == null)
            {
                return null;
            }
            var extractsLine = extractsLineBase.DepthClone<ExtractsLine>();
            if (extractsLine != null)
            {
                extractsLine.DbId = entry.DbId;
                extractsLine.EntryName = entry.Name;
                extractsLine.EntryId = entry.EntryId;
            }
            return extractsLine;
        }
    }
}
