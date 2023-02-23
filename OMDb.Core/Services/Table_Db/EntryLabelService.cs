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
        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelDb> SelectAllEntryLabel()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select * from EntryLabel ");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelDb>(sb.ToString());
        }


        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelDb> SelectAllEntryLabel(string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($@"select * from EntryLabel where DbId='{dbId}'");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelDb>(sb.ToString());
        }



        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void AddEntryLabel(EntryLabelDb eldb)
        {
            DbService.LocalDb.Insertable(eldb).ExecuteCommand();
        }
    }
}
