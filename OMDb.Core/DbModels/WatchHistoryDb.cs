﻿using SqlSugar;
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
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 词条本地唯一Id
        /// </summary>
        public string EntryId { get; set; }
        public DateTime Time { get; set; }
        /// <summary>
        /// 观看完毕，作为有效观看次数进行统计
        /// </summary>
        public bool Done { get; set; }
        /// <summary>
        /// 观看备注
        /// 可以用来记录本次看到了哪
        /// 写给自己看的
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Mark { get; set; }
    }
}
