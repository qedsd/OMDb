using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class StorageSize
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long UsedByte { get; set; }
        public long UsableByte { get; set; }
    }
}
