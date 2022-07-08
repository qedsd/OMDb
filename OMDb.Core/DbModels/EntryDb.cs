using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 存储词条
    /// </summary>
    [SugarTable("Entry")]
    public class EntryDb
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 文件夹存储路径
        /// 当前数据库相对路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 词条创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最近观看时间
        /// </summary>
        public DateTime? LastWatchTime { get; set; }
        /// <summary>
        /// 最新更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// 观看次数
        /// </summary>
        public int WatchTimes { get; set; }
        /// <summary>
        /// 我的评分
        /// </summary>
        public double? MyRating { get; set; }
    }
}
