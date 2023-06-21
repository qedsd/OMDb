using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class WatchHistory:DbModels.EntryWatchHistoryDb
    {
        /// <summary>
        /// 所属数据库唯一标识
        /// </summary>
        public string DbId { get; set; }
        public static WatchHistory Create(DbModels.EntryWatchHistoryDb dbItem, string DbId)
        {
            var newItem = dbItem.DepthClone<WatchHistory>();
            if (newItem != null)
            {
                newItem.DbId = DbId;
            }
            return newItem;
        }
    }
}
