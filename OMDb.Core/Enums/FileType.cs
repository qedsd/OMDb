using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Enums
{
    /// <summary>
    /// 文件類型
    /// </summary>
    public enum FileType
    {
        [Description("文件夾")]
        Folder,
        [Description("圖片文件")]
        Img,
        [Description("視頻文件")]
        Video,
        [Description("視頻文件")]
        Audio,
        [Description("視頻文件")]
        Sub,
        [Description("其他文件")]
        More,
        [Description("全部文件")]
        All,
        [Description("全部文件(夾)")]
        TotalAll
    }
}
