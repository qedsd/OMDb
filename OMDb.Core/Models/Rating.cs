using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 评分
    /// </summary>
    public class Rating
    {
        public double Max { get; set; }
        public double Min { get; set; } = 0;
        public double Rate { get; set; }
        public string Remarks { get; set; }
        public Rating(double rate, double max)
        {
            Max = max;
            Rate = rate;
        }
        public Rating(double rate, double max, double min)
        {
            Max = max;
            Rate = rate;
            Rate = rate;
        }
        public Rating( double rate, double max, double min, string remarks)
        {
            Max = max;
            Rate = rate;
            Rate = rate;
            Remarks = remarks;
        }
    }
}
