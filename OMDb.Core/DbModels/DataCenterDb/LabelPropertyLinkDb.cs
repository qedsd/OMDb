using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 关联词条与标签
    /// </summary>
    [SugarTable("LabelPropertyLKDb")]
    public class LabelPropertyLinkDb
    {
        /// <summary>
        /// 词条所在的dbid
        /// </summary>
        public string DbCenterId { get; set; }
        public string LPIdA { get; set; }
        public string LPIdB { get; set; }
    }
}
