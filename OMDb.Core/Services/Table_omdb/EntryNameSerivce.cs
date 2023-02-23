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
                var names = DbService.GetConnection(dbId).Queryable<DbModels.EntryNameDb>().Where(p => p.EntryId == entryId).ToList();
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
            var names = DbService.GetConnection(entry.DbId).Queryable<EntryNameDb>().Where(p => p.EntryId == entry.EntryId).ToList();
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
            var names = DbService.GetConnection(dbId).Queryable<EntryNameDb>().Where(p => p.IsDefault && ids.Contains(p.EntryId)).ToList();
            if (names.Any())
            {
                return names.ToDictionary(p => p.EntryId, p => p.Name);
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
            var dic = QueryName(entries.Select(p => p.EntryId), dbId);
            if (dic != null && dic.Any())
            {
                foreach (var entry in entries)
                {
                    if (dic.TryGetValue(entry.EntryId, out string name))
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
            var entryNameDbs = await Task.Run(() => DbService.GetConnection(dbId).Queryable<EntryNameDb>().Where(p => p.EntryId == id).ToList());
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
            await Task.Run(()=>DbService.GetConnection(dbId).Insertable(entryNames).ExecuteCommand());
        }

        public static async Task RemoveNamesAsync(string id, string dbId)
        {
            await Task.Run(() => DbService.GetConnection(dbId).Deleteable<EntryNameDb>(p=>p.EntryId == id).RemoveDataCache().ExecuteCommand());
        }
        //public static async Task RemoveNamesAsync(string id, string dbId)
        //{
        //    await Task.Run(() => DbService.GetConnection(dbId).Deleteable<EntryNameDb>(p => p.EntryId == id).ExecuteCommand());
        //}
        /// <summary>
        /// 若词条存在默认名称，则更新，负责插入
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task UpdateOrAddDefaultNamesAsync(string id, string dbId,string name)
        {
            await Task.Run( () =>
            {
                UpdateOrAddDefaultNames(id, dbId, name);
            });
        }

        /// <summary>
        /// 若词条存在默认名称，则更新，负责插入
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void UpdateOrAddDefaultNames(string id, string dbId, string name)
        {
            var defaultName = DbService.GetConnection(dbId).Queryable<EntryNameDb>().First(p => p.EntryId == id && p.IsDefault);
            if (defaultName == null)//插入
            {
                EntryNameDb entryNameDb = new EntryNameDb()
                {
                    Name = name,
                    EntryId = id,
                    IsDefault = true
                };
                DbService.GetConnection(dbId).Insertable(entryNameDb).RemoveDataCache().ExecuteCommand();
            }
            else//更新
            {
                defaultName.Name = name;
                DbService.GetConnection(dbId).Updateable(defaultName).RemoveDataCache().ExecuteCommand();
            }
        }

        /// <summary>
        /// 模糊搜索词条名
        /// </summary>
        /// <param name="partstr"></param>
        /// <returns>id为EntryDb.Id，Value为EntryNameDb.Name</returns>
        public static async Task<List<QueryResult>> QueryLikeNamesAsync(string partstr)
        {
            List<QueryResult> queryItems = new List<QueryResult>();
            foreach(var db in DbService.Dbs)
            {
                var ls = await db.Value.Queryable<EntryNameDb>().Where(p=>p.Name.Contains(partstr)).ToListAsync();
                if(ls != null)
                {
                    foreach(var item in ls)
                    {
                        queryItems.Add(new QueryResult(item.EntryId, item.Name,db.Key));
                    }
                }
            }
            return queryItems;
        }
        /// <summary>
        /// 完整搜索词条名
        /// </summary>
        /// <param name="partstr"></param>
        /// <returns>id为EntryId，Value为EntryNameDb.Name</returns>
        public static async Task<List<QueryResult>> QueryFullNamesAsync(string name)
        {
            List<QueryResult> queryItems = new List<QueryResult>();
            foreach (var db in DbService.Dbs)
            {
                var ls = await db.Value.Queryable<EntryNameDb>().Where(p => p.Name == name).ToListAsync();
                if (ls != null)
                {
                    foreach (var item in ls)
                    {
                        queryItems.Add(new QueryResult(item.EntryId, item.Name, db.Key));
                    }
                }
            }
            return queryItems;
        }




        public static void AddEntryName(List<EntryNameDb> endbs, string dbId)
        {
             DbService.GetConnection(dbId).Insertable(endbs).ExecuteCommand();
        }


    }
}
