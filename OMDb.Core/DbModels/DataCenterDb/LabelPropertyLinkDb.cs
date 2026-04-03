using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 关联属性标签关联
    /// </summary>
    [SugarTable("LabelPropertyLink")]
    public class LabelPropertyLinkDb
    {
        public string LPIDA { get; set; }
        public string LPIDB { get; set; }
    }
}
