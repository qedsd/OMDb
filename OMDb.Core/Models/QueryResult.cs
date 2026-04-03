using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 查询结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryResult
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        public string Id { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// 所属数据库唯一标识
        /// </summary>
        public string DbId { get; set; }
        public QueryResult(string id, object value, string dbId)
        {
            Id = id;
            Value = value;
            DbId = dbId;
        }
        public QueryItem ToQueryItem()
        {
            return new QueryItem(Id,DbId);
        }
    }
}
