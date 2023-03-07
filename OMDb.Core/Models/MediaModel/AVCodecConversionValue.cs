using OMDb.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class ACodecConversionValue
    {
        public int StreamIndex { get; set; }
        public Dictionary<ACodecConversion, dynamic> Values { get; set; }
    }
    public class VCodecConversionValue
    {
        public int StreamIndex { get; set; }
        public Dictionary<VCodecConversion, dynamic> Values { get; set; }
    }
}
