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
    [SugarTable("EntryName")]
    public class EntryNameDb
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 词条本地唯一Id
        /// </summary>
        public string EntryId { get; set; }
        /// <summary>
        /// 词条名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// 如语言类型
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Mark { get; set; }
        /// <summary>
        /// 是否默认显示
        /// </summary>
        public bool IsDefault { get; set; }
        public EntryNameDb()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
