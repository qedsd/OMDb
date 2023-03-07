using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// Db源
    /// 每个Db源有各自的标签、仓库、词条明细
    /// </summary>
    [SugarTable("DbSource")]
    public class DbSourceDb
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string DbName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime ModifyTime { get; set; }
    }
}
