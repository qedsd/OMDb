using OMDb.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class SortModel
    {
        public SortModel()
        {
            SortType = SortType.CreateTime;
            SortWay =SortWay.Positive;
        }
        public SortModel(SortType st, SortWay sw)
        {
            SortType = st;
            SortWay = sw;
        }
        public SortType SortType { get; set; }

        public SortWay SortWay { get; set; }
    }
}
