using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class RecentEntry
    {
        public RecentFile RecentFile { get; set; }
        public Entry Entry { get; set; }
    }
}
