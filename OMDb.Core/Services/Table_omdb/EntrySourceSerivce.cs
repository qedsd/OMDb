using OMDb.Core.DbModels;
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
        public static List<EntrySourceDb> QueryPath(string entryId, string fileType, string dbId)
        {
            if (!string.IsNullOrEmpty(entryId))
            {
                if (fileType.Equals("1")) { var folder = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("1")).ToList(); return folder; }
                if (fileType.Equals("2")) { var lstImg = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("2")).ToList(); return lstImg; }
                if (fileType.Equals("3")) { var lstVid = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("3")).ToList(); return lstVid; }
                if (fileType.Equals("4")) { var lstAud = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("4")).ToList(); return lstAud; }
                if (fileType.Equals("5")) { var lstMore = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("5")).ToList(); return lstMore; }
                if (fileType.Equals("6")) { var lstFiles = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("6")).ToList(); return lstFiles; }
            }
            return null;
        }


        public static void SavePath(List<EntrySourceDb> lstEsDb, string dbId)
        {
            if (lstEsDb.Count>0)
            {
                DbService.GetConnection(dbId).Insertable<EntrySourceDb>(lstEsDb).ExecuteCommand();
            }
        }
    }
}
