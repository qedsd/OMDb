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
        public static List<EntrySourceDb> SelectEntrySource(string entryId, string dbId)
        {
            if (!string.IsNullOrEmpty(entryId))
            {
                var param = string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"select * from EntrySource where EntryId='{0}'", entryId, param);
                var result = DbService.GetConnection(dbId).Ado.SqlQuery<EntrySourceDb>(sb.ToString());
                return result;
            }
            return null;
        }


        public static void AddEntrySource(List<EntrySourceDb> lstEsDb, string dbId)
        {
            if (lstEsDb.Count > 0)
            {
                DbService.GetConnection(dbId).Insertable<EntrySourceDb>(lstEsDb).ExecuteCommand();
            }
        }
        public static void AddEntrySource(string entryId, List<string> lstEsPath, string dbId, PathType fileType = PathType.More)
        {
            if (lstEsPath.Count > 0)
            {
                foreach (var item in lstEsPath)
                {
                    EntrySourceDb esdb = new EntrySourceDb()
                    {
                        EntryId = entryId,
                        Path = item,
                    };
                    /*switch (fileType)
                    {
                        case PathType.Folder:
                            esdb.PathType = "Folder";
                            break;
                        case PathType.Image:
                            esdb.PathType = "Image";
                            break;
                        case PathType.Video:
                            esdb.PathType = "Video";
                            break;
                        case PathType.Audio:
                            esdb.PathType = "Audio";
                            break;
                        case PathType.VideoSub:
                            esdb.PathType = "VideoSub";
                            break;
                        case PathType.More:
                            esdb.PathType = "More";
                            break;
                        default:
                            esdb.PathType = "More";
                            break;
                    }*/
                    DbService.GetConnection(dbId).Insertable<EntrySourceDb>(esdb).ExecuteCommand();
                }
            }
        }
        public static void AddEntrySource_PathFolder(string entryId, string EsPath, string dbId)
        {
            EntrySourceDb esdb = new EntrySourceDb()
            {
                EntryId = entryId,
                Path = EsPath,
                PathType=PathType.Folder
            };
            DbService.GetConnection(dbId).Insertable<EntrySourceDb>(esdb).ExecuteCommand();

        }
        public static void UpdateEntrySource_PathFolder(string entryId, string PathFolder, string dbId)
        {
            //有可能從其他存儲模式轉爲指定文件夾，此時需要判斷是否需要新增
            StringBuilder sb = new StringBuilder();
            EntrySourceDb esdb = new EntrySourceDb()
            {
                EntryId = entryId,
                Path = PathFolder,
            };
            sb.AppendFormat(@"insert into entrySource (Id,entryid,path,PathType) select '{0}','{1}','{2}',1
where not exists (SELECT 1 FROM entrySource where entryid = '{1}' and PathType=0 );", esdb.Id, esdb.EntryId, esdb.Path); 
            sb.AppendFormat(@"update EntrySource set Path='{0}' where EntryId='{1}' and PathType=0", PathFolder, entryId);
            var result = DbService.GetConnection(dbId).Ado.ExecuteCommand(sb.ToString());
        }
        public static void RemoveEntrySource_PathFiles(string entryId, List<string> PathFiles, string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"delete from EntrySource where EntryId='{1}' and Path in'{}'", string.Join("','",PathFiles), entryId);
            var result = DbService.GetConnection(dbId).Ado.SqlQuery<EntrySourceDb>(sb.ToString());
        }
    }
}
