using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 摘抄台词
    /// 保存至词条文件夹内
    /// </summary>
    public class ExtractsLineBase
    {
        /// <summary>
        /// 台词内容
        /// MD格式
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 台词来源
        /// 非词条名自定义内容
        /// 如 S01 01:02:03
        /// </summary>
        public string From { get; set; }
    }
}
