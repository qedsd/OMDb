using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 观看记录
    /// </summary>
    [SugarTable("WatchHistory")]
    public class WatchHistoryDb
    {
        /// <summary>
        /// 词条本地唯一Id
        /// </summary>
        public string Id { get; set; }
        public DateTime Time { get; set; }
        /// <summary>
        /// 观看备注
        /// 可以用来记录本次看到了哪
        /// 写给自己看的
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Mark { get; set; }
    }
}
