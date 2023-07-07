using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class Entry: DbModels.EntryDb
    {
        /// <summary>
        /// 所属数据库唯一标识
        /// </summary>
        public string DbId { get; set; }
        public string Name { get; set; }
        public int ReleaseYear
        {
            get=> ReleaseDate.HasValue ? ReleaseDate.Value.Year : 1895;
        }
        public static Entry Create(DbModels.EntryDb entryDb,string DbId)
        {
            var entry = entryDb.DepthClone<Entry>();
            if (entry != null)
            {
                entry.DbId = DbId;
            }
            return entry;
        }
    }
}
