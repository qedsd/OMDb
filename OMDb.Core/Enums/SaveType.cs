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
    /// 存儲模式
    /// </summary>
    public enum SaveType
    {
        [Description("指定文件夾")]
        Folder,
        [Description("指定文件")]
        Files,
        [Description("本地存儲")]
        Local,
    }
}
