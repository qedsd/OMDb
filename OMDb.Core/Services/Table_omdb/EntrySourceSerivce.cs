using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntrySourceSerivce
    {

        /// <summary>
        /// 查詢文件路徑
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntrySourceDb> SelectEntrySource(string entryId, string dbId, Enums.FileType fileType = FileType.All)
        {
            if (!string.IsNullOrEmpty(entryId))
            {
                var param = string.Empty;
                switch (fileType)
                {
                    case FileType.Folder:
                        param = @"='1'";
                        break;
                    case FileType.Img:
                        param = @"='2'";
                        break;
                    case FileType.Video:
                        param = @"='3'";
                        break;
                    case FileType.Audio:
                        param = @"='4'";
                        break;
                    case FileType.Sub:
                        param = @"='6'";
                        break;
                    case FileType.More:
                        param = @"='7'";
                        break;
                    case FileType.All:
                        param = @"in ('2','3','4','5','6','7')";
                        break;
                    case FileType.TotalAll:
                        param = @"in ('1','2','3','4','5','6','7')";
                        break;
                    default:
                        param = @"in ('1','2','3','4','5','6','7')";
                        break;
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"select * from EntrySource where EntryId={0} and FileType {1}", entryId, param);
                var result = DbService.GetConnection(dbId).Ado.SqlQuery<EntrySourceDb>(sb.ToString());
                return result;
                //var folder = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("1")).ToList();
            }
            return null;
        }


        public static void InsertEntrySource(List<EntrySourceDb> lstEsDb, string dbId)
        {
            if (lstEsDb.Count > 0)
            {
                DbService.GetConnection(dbId).Insertable<EntrySourceDb>(lstEsDb).ExecuteCommand();
            }
        }
    }
}
