using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryLabelPropertyService
    {
        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelPropertyLKDb> SelectAllEntryLabel()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select * from EntryLabel ");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelPropertyLKDb>(sb.ToString());
        }


        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelPropertyLKDb> SelectAllEntryLabel(string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($@"select * from EntryLabel where DbId='{dbId}'");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelPropertyLKDb>(sb.ToString());
        }



        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void AddEntryLabel(EntryLabelPropertyLKDb eldb)
        {
            DbService.LocalDb.Insertable(eldb).ExecuteCommand();
        }
    }
}
