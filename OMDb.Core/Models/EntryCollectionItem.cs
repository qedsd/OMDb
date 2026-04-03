using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class EntryCollectionItem: EntryCollectionItemDb
    {
        public string AddDate
        {
            get=> AddTime.ToShortDateString();
        }
        public Entry Entry { get; set; }
        public static EntryCollectionItem Create(EntryCollectionItemDb entryCollectionItemDb)
        {
            return entryCollectionItemDb.DepthClone<EntryCollectionItem>();
        }
    }
}
