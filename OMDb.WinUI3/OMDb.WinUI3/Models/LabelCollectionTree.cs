using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    internal class LabelCollectionTree
    {
        public LabelCollection LabelCollection { get; set; }
        public List<LabelCollection> Children { get; set; }
    }
}
