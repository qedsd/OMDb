using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class FilterModel
    {
        public FilterModel()
        {
            LabelClassIds = new List<string>();
            LabelPropertyIds = new List<string>();
            StorageIds = new List<string>();
            CreateDateBegin = DateTime.Now;
            CreateDateEnd = DateTime.Now;
            BusinessDateBegin = DateTime.Now;
            BusinessDateEnd = DateTime.Now;
            RateMin = 0.0;
            RateMax = 5.0;
        }

        public List<string> LabelClassIds { get; set; }

        public List<string> LabelPropertyIds { get; set; }

        public List<string> StorageIds { get; set; }

        public DateTime CreateDateBegin { get; set; }
        public DateTime CreateDateEnd { get; set; }


        public DateTime BusinessDateBegin { get; set; }
        public DateTime BusinessDateEnd { get; set; }

        public double RateMin { get; set; }
        public double RateMax { get; set; }



    }
}
