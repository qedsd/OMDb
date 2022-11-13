using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 词条集锦关联的词条id
    /// </summary>
    [SugarTable("EntryCollections")]
    public class EntryCollectionItemDb
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 词条id
        /// </summary>
        public string EntryId { get; set; }
        /// <summary>
        /// EntryCollectionDb主键
        /// </summary>
        public string CollectionId { get; set; }
        /// <summary>
        /// 词条所在的dbid
        /// </summary>
        public string DbId { get; set; }
    }
}
