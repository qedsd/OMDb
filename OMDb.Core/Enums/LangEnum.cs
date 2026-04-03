using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Enums
{
    public enum LangEnum
    {
        [Description("中文")]
        zh_CN,
        [Description("英语")]
        us_US,
        [Description("日语")]
        jp_JP,
        [Description("法语")]
        fr_FR,
        [Description("德语")]
        de_DE
    }
}