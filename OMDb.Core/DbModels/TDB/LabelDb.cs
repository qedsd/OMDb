using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 标签->类型
    /// 每个数据库都保存一份，数据统一
    /// </summary>
    [SugarTable("Label")]
    public class LabelDb
    {
        /// <summary>
        /// 内码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string LCId { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 父亲Id
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ParentId { get; set; }

        /// <summary>
        /// 所属媒体库Id
        /// </summary>
        public string DbSourceId { get; set; }

        /// <summary>
        /// 是否在分类页签显示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public int Seq { get; set; }
    }
}
