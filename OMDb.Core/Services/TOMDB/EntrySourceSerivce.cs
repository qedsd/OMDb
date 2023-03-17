﻿using OMDb.Core.DbModels;
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
        public static List<EntrySourceDb> SelectEntrySource(string entryId, string dbId, Enums.FileType fileType = FileType.TotalAll)
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
                sb.AppendFormat(@"select * from EntrySource where EntryId='{0}' and FileType {1}", entryId, param);
                var result = DbService.GetConnection(dbId).Ado.SqlQuery<EntrySourceDb>(sb.ToString());
                return result;
                //var folder = DbService.GetConnection(dbId).Queryable<DbModels.EntrySourceDb>().Where(p => p.EntryId == entryId).Where(a => a.FileType.Equals("1")).ToList();
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
        public static void AddEntrySource(string entryId, List<string> lstEsPath, string dbId, FileType fileType = FileType.More)
        {
            if (lstEsPath.Count > 0)
            {
                //下個版本臨時表處理
                foreach (var item in lstEsPath)
                {
                    EntrySourceDb esdb = new EntrySourceDb()
                    {
                        EntryId = entryId,
                        Path = item,
                    };
                    switch (fileType)
                    {
                        case FileType.Folder:
                            esdb.FileType = '1';
                            break;
                        case FileType.Img:
                            esdb.FileType = '2';
                            break;
                        case FileType.Video:
                            esdb.FileType = '3';
                            break;
                        case FileType.Audio:
                            esdb.FileType = '4';
                            break;
                        case FileType.Sub:
                            esdb.FileType = '5';
                            break;
                        case FileType.More:
                            esdb.FileType = '6';
                            break;
                        default:
                            esdb.FileType = '6';
                            break;
                    }
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
                FileType= '1',
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
            sb.AppendFormat(@"insert into entrySource (Id,entryid,path,filetype) select '{0}','{1}','{2}',1
where not exists (SELECT 1 FROM entrySource where entryid = '{1}' and FileType=1 );", esdb.Id, esdb.EntryId, esdb.Path); 
            sb.AppendFormat(@"update EntrySource set Path='{0}' where EntryId='{1}' and FileType=1", PathFolder, entryId);
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