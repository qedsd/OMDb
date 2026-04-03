using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class EntryCollection : EntryCollectionDb
    {
        public List<EntryCollectionItem> Items { get; set; }

        public EntryCollection()
        {

        }

        public static EntryCollection Create(EntryCollectionDb entryCollectionDb)
        {
            return entryCollectionDb.DepthClone<EntryCollection>();
        }

        public EntryCollectionDb ToEntryCollectionDb()
        {
            return new EntryCollectionDb()
            {
                Id = Id,
                Title = Title,
                Description = Description,
                CreateTime = CreateTime,
                LastUpdateTime = LastUpdateTime,
            };
        }
    }
}
