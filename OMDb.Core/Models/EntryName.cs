using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class EntryName : DbModels.EntryNameDb
    {
        /// <summary>
        /// 所属数据库唯一标识
        /// </summary>
        public string DbId { get; set; }
        public static EntryName Create(DbModels.EntryNameDb dbItem, string DbId)
        {
            var newItem = dbItem.DepthClone<EntryName>();
            if (newItem != null)
            {
                newItem.DbId = DbId;
            }
            return newItem;
        }
    }
}
