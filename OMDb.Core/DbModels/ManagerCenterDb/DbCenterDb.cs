using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels.ManagerCenterDb
{
    /// <summary>
    /// 数据中心
    /// 每个数据中心有各自的标签、仓库、词条明细
    /// </summary>
    [SugarTable("DbCenter")]
    public class DbCenterDb
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
