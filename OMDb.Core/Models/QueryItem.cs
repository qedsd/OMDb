using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 批量查询Entry
    /// </summary>
    public class QueryItem
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所属数据库唯一标识
        /// </summary>
        public string DbId { get; set; }
        public QueryItem(string id, string dbId)
        {
            Id = id;
            DbId = dbId;
        }
    }
}
