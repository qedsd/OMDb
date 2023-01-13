using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OMDb.Core.DbModels
{
    /// <summary>
    /// 词条集锦
    /// </summary>
    [SugarTable("Config")]
    public class ConfigDb
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        public Blob ConfigXml { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }
    }
}
