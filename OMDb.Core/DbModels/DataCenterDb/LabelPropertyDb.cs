using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 标签->属性
    /// 每个数据库都保存一份，数据统一
    /// </summary>
    [SugarTable("LabelProperty")]
    public class LabelPropertyDb
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string LPId { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ParentId { get; set; }
        public string DbCenterId { get; set; }
        public int Level { get; set; }
        public int Seq { get; set; }
    }
}
