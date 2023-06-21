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
    /// 词条名称
    /// 一个词条可能存在多个不同语言名称
    /// </summary>
    [SugarTable("EntrySource")]
    public class EntrySourceDb
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 词条本地唯一Id
        /// </summary>
        public string EntryId { get; set; }



        /// <summary>
        /// 指向文件(夹)地址
        /// </summary>

        public string Path { get; set; }

        public PathType PathType { get; set; }

        public EntrySourceDb()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
