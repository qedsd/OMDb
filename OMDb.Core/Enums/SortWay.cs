using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Enums
{
    /// <summary>
    /// 正序反序排序
    /// </summary>
    public enum SortWay
    {
        [Description("正序")]
        Positive,
        [Description("反序")]
        Reverse
    }
}
