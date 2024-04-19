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
    [SugarTable("EntryLabelLink")]
    public class EntryLabelLinkDb
    {
        /// <summary>
        /// 词条所在的dbid
        /// </summary>
        public string DbId { get; set; }
        public string EntryId { get; set; }
        public string LabelID { get; set; }
    }
}
