using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 存储仓库
    /// </summary>
    [SugarTable("Storage")]
    public class StorageDb
    {
        /// <summary>
        /// 仓库内码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string StorageName { get; set; }
        /// <summary>
        /// 文件夹存储路径
        /// 当前数据库相对路径
        /// </summary>
        public string StoragePath { get; set; }
        /// <summary>
        /// 封面图片
        /// 相对词条Img路径、
        /// 一般为词条img文件夹下的文件名
        /// </summary>
        public string CoverImg { get; set; }
        /// <summary>
        /// 仓库创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 词条数
        /// </summary>
        public int? EntryCount { get; set; }
    }
}
