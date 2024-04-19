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
            CreateDateBegin = Convert.ToDateTime("1970-01-01");
            CreateDateEnd = DateTime.Now;
            BusinessDateBegin = Convert.ToDateTime("1970-01-01");
            BusinessDateEnd = DateTime.Now;
            RateMin = 0.0;
            RateMax = 5.0;
            IsFilterLabelClass = false;
            IsFilterLabelProperty = false;
            IsFilterStorage = false;
        }

        /// <summary>
        /// 过滤分类标签集合
        /// </summary>
        public List<string> LabelClassIds { get; set; }
        /// <summary>
        /// 是否过滤分类标签
        /// </summary>
        public bool IsFilterLabelClass { get; set; }

        /// <summary>
        /// 过滤属性标签集合
        /// </summary>
        public List<string> LabelPropertyIds { get; set; }
        /// <summary>
        /// 是否过滤属性标签
        /// </summary>
        public bool IsFilterLabelProperty { get; set; }

        /// <summary>
        /// 过滤仓库集合
        /// </summary>
        public List<string> StorageIds { get; set; }
        /// <summary>
        /// 是否过滤仓库
        /// </summary>
        public bool IsFilterStorage { get; set; }

        public DateTime CreateDateBegin { get; set; }
        public DateTime CreateDateEnd { get; set; }


        public DateTime BusinessDateBegin { get; set; }
        public DateTime BusinessDateEnd { get; set; }

        public double RateMin { get; set; }
        public double RateMax { get; set; }



    }
}
