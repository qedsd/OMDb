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
    [SugarTable("EntryLabel")]
    public class EntryLabelDb
    {
        public string EntryId { get; set; }
        public string LabelId { get; set; }
    }
}
