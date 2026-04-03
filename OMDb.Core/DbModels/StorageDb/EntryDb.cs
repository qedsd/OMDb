using OMDb.Core.Enums;
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
        [SugarColumn(IsPrimaryKey = true)]
        public string EntryId { get; set; }
        /// <summary>
        /// 文件夹存储路径
        /// 当前数据库相对路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 封面图片
        /// 相对词条Img路径、
        /// 一般为词条img文件夹下的文件名
        /// </summary>
        public string CoverImg { get; set; }
        /// <summary>
        /// 词条创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最近观看时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime LastWatchTime { get; set; }
        /// <summary>
        /// 最新更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// 观看次数
        /// </summary>
        public int WatchTimes { get; set; } = 0;
        /// <summary>
        /// 上映日期
        /// </summary>
        public DateTimeOffset? ReleaseDate { get; set; }
        /// <summary>
        /// 我的评分
        /// </summary>
        public double MyRating { get; set; }


        /// <summary>
        /// 类型 1:指向文件夹 2:指向文件 3:本地存储
        /// </summary>
        public SaveType SaveType { get; set; }

        //资源文件统计
        public int CountVideo { get; set; } = 0;
        public int CountImage { get; set; } = 0;
        public int CountAudio { get; set; } = 0;
    }
}
