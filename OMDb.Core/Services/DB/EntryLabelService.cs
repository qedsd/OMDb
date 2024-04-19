using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryLabelService
    {
        public static List<EntryLabelLinkDb> SelectAllEntryLabel()
        {
            return DbService.ConfigDb.Queryable<EntryLabelLinkDb>().ToList();
        }

        public static List<EntryLabelLinkDb> SelectAllEntryLabel(string dbId)
        {
            return DbService.ConfigDb.Queryable<EntryLabelLinkDb>().Where(p=>p.DbId == dbId).ToList();
        }

        /// <summary>
        /// 添加Entry&Label 关联关系
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void AddEntryLabel(Entry entry, LabelDb label)
        {
            EntryLabelLinkDb entryLabelLinkDb = new EntryLabelLinkDb()
            {
                DbId = entry.DbId,
                EntryId = entry.EntryId,
                LabelID = label.ID
            };
            DbService.ConfigDb.Insertable(entryLabelLinkDb).ExecuteCommand();
        }
    }
}
