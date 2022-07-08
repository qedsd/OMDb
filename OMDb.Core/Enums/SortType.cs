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
        [Description("词条创建时间")]
        CreateTime,
        [Description("最近观看")]
        LastWatchTime,
        [Description("我的评分")]
        MyRating,
        [Description("观看次数")]
        WatchTimes,
        [Description("最近更新")]
        LastUpdateTime,
    }
}
