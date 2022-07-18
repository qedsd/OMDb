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
    public static class EntryNameSerivce
    {
        /// <summary>
        /// 获取词条默认名称
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task<string> QueryNameAsync(string entryId, string dbId)
        {
            return await Task.Run(() => QueryName(entryId, dbId));
        }
        /// <summary>
        /// 获取词条默认名称
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static string QueryName(string entryId, string dbId)
        {
            if (!string.IsNullOrEmpty(entryId))
            {
                var names = DbService.Db.GetConnection(dbId).Queryable<DbModels.EntryNameDb>().Where(p => p.Id == entryId).ToList();
                if (names != null && names.Any())
                {
                    return names.FirstOrDefault(p => p.IsDefault)?.Name;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取词条默认名称
        /// 无默认名称时将使用第一个名称
        /// </summary>
        /// <param name="entry"></param>
        public static void SetName(Entry entry)
        {
            var names = DbService.Db.GetConnection(entry.DbId).Queryable<EntryNameDb>().Where(p => p.Id == entry.Id).ToList();
            if (names != null && names.Any())
            {
                var name = names.FirstOrDefault(p => p.IsDefault);
                if (name == null)
                {
                    entry.Name = names.First().Name;
                }
                else
                {
                    entry.Name = name.Name;
                }
            }
        }

        /// <summary>
        /// 获取默认名字
        /// </summary>
        /// <param name="ids">词条id集</param>
        /// <param name="dbId">数据库id</param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryName(IEnumerable<string> ids, string dbId)
        {
            var names = DbService.Db.GetConnection(dbId).Queryable<EntryNameDb>().Where(p => p.IsDefault && ids.Contains(p.Id)).ToList();
            if (names.Any())
            {
                return names.ToDictionary(p => p.Id, p => p.Name);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 设置词条默认显示名称
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="dbId"></param>
        public static void SetName(IEnumerable<Entry> entries, string dbId)
        {
            var dic = QueryName(entries.Select(p => p.Id), dbId);
            if (dic != null && dic.Any())
            {
                foreach (var entry in entries)
                {
                    if (dic.TryGetValue(entry.Id, out string name))
                    {
                        entry.Name = name;
                    }
                }
            }
        }

        /// <summary>
        /// 查询词条所有名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task<List<EntryName>> QueryNamesAsync(string id, string dbId)
        {
            var entryNameDbs = await Task.Run(() => DbService.Db.GetConnection(dbId).Queryable<EntryNameDb>().Where(p => p.Id == id).ToList());
            if (entryNameDbs != null && entryNameDbs.Any())
            {
                return entryNameDbs.Select(p => EntryName.Create(p, dbId)).ToList();
            }
            else
            {
                return null;
            }
        }

        public static async Task AddNamesAsync(List<EntryNameDb> entryNames, string dbId)
        {
            await Task.Run(()=>DbService.Db.GetConnection(dbId).Insertable(entryNames).ExecuteCommand());
        }

        public static async Task RemoveNamesAsync(string id, string dbId)
        {
            await Task.Run(() => DbService.Db.GetConnection(dbId).Deleteable<EntryNameDb>(p=>p.Id == id).ExecuteCommand());
        }
    }
}
