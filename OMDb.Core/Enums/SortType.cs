using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Enums
{
    public enum SortType
    {
        [Description("创建日期")]
        CreateTime,
        [Description("上映日期")]
        BusinessDate,
        [Description("评分")]
        MyRating,
        [Description("观看日期")]
        LastWatchTime,
        [Description("观看次数")]
        WatchTimes,
        [Description("更新日期")]
        LastUpdateTime,
    }
}
