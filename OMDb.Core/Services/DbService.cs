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
    public static class DbService
    {
        private static SqlSugarScope Db;
        private static readonly HashSet<string> DbConfigIds = new();
        static DbService()
        {
            
        }
        /// <summary>
        /// 按数据库连接字符串创建多租户
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        public static bool AddDb(string connet,string configId)
        {
            if (DbConfigIds.Add(configId))
            {
                if (Db == null)
                {
                    Db = new SqlSugarScope(new ConnectionConfig()
                    {
                        ConnectionString = connet,
                        DbType = DbType.Sqlite,
                        IsAutoCloseConnection = true,
                        ConfigId = configId
                    });
                }
                else
                {
                    Db.AddConnection(new ConnectionConfig()
                    {
                        ConnectionString = connet,
                        DbType = DbType.Sqlite,
                        IsAutoCloseConnection = true,
                        ConfigId = configId
                    });
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 按指定排序方式获取词条
        /// 获取完自行处理分页
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="sortWay"></param>
        /// <returns></returns>
        public static List<QueryResult> QueryEntry(Enums.SortType sortType,Enums.SortWay sortWay)
        {
            if(DbConfigIds?.Count == 0)
            {
                return null;
            }
            else
            {
                return sortType switch
                {
                    Enums.SortType.CreateTime => SortByCreateTime(sortWay),
                    Enums.SortType.LastWatchTime => SortByLastWatchTime(sortWay),
                    Enums.SortType.LastUpdateTime => SortByLastUpdateTime(sortWay),
                    Enums.SortType.WatchTimes => SortByWatchTimes(sortWay),
                    Enums.SortType.MyRating => SortByMyRating(sortWay),
                    _ => null,
                };
            }
        }

        private static List<QueryResult> SortByCreateTime(Enums.SortWay sortWay)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            foreach (var dbid in DbConfigIds)
            {
                var db = Db.GetConnection(dbid);
                if (db != null)
                {
                    if (sortWay == Enums.SortWay.Positive)
                    {
                        var ls = db.Queryable<DbModels.EntryDb>()
                            .OrderBy(p=>p.CreateTime)
                            .Select(p=>new {p.Id,p.CreateTime})
                            .ToList();
                        ls.ForEach(p =>
                        {
                            queryResults.Add(new QueryResult(p.Id, p.CreateTime, dbid));
                        });
                    }
                }
            }
            if (sortWay == Enums.SortWay.Positive)
            {
                return queryResults.OrderBy(p => p.Value).ToList();
            }
            else
            {
                return queryResults.OrderByDescending(p => p.Value).ToList();
            }
        }
        private static List<QueryResult> SortByLastWatchTime(Enums.SortWay sortWay)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            foreach (var dbid in DbConfigIds)
            {
                var db = Db.GetConnection(dbid);
                if (db != null)
                {
                    if (sortWay == Enums.SortWay.Positive)
                    {
                        var ls = db.Queryable<DbModels.EntryDb>()
                            .OrderBy(p => p.CreateTime)
                            .Select(p => new { p.Id, p.LastWatchTime })
                            .ToList();
                        ls.ForEach(p =>
                        {
                            queryResults.Add(new QueryResult(p.Id, p.LastWatchTime, dbid));
                        });
                    }
                }
            }
            if (sortWay == Enums.SortWay.Positive)
            {
                return queryResults.OrderBy(p => p.Value).ToList();
            }
            else
            {
                return queryResults.OrderByDescending(p => p.Value).ToList();
            }
        }
        private static List<QueryResult> SortByLastUpdateTime(Enums.SortWay sortWay)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            foreach (var dbid in DbConfigIds)
            {
                var db = Db.GetConnection(dbid);
                if (db != null)
                {
                    if (sortWay == Enums.SortWay.Positive)
                    {
                        var ls = db.Queryable<DbModels.EntryDb>()
                            .OrderBy(p => p.CreateTime)
                            .Select(p => new { p.Id, p.LastUpdateTime })
                            .ToList();
                        ls.ForEach(p =>
                        {
                            queryResults.Add(new QueryResult(p.Id, p.LastUpdateTime, dbid));
                        });
                    }
                }
            }
            if (sortWay == Enums.SortWay.Positive)
            {
                return queryResults.OrderBy(p => p.Value).ToList();
            }
            else
            {
                return queryResults.OrderByDescending(p => p.Value).ToList();
            }
        }
        private static List<QueryResult> SortByWatchTimes(Enums.SortWay sortWay)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            foreach (var dbid in DbConfigIds)
            {
                var db = Db.GetConnection(dbid);
                if (db != null)
                {
                    if (sortWay == Enums.SortWay.Positive)
                    {
                        var ls = db.Queryable<DbModels.EntryDb>()
                            .OrderBy(p => p.CreateTime)
                            .Select(p => new { p.Id, p.WatchTimes })
                            .ToList();
                        ls.ForEach(p =>
                        {
                            queryResults.Add(new QueryResult(p.Id, p.WatchTimes, dbid));
                        });
                    }
                }
            }
            if (sortWay == Enums.SortWay.Positive)
            {
                return queryResults.OrderBy(p => p.Value).ToList();
            }
            else
            {
                return queryResults.OrderByDescending(p => p.Value).ToList();
            }
        }
        private static List<QueryResult> SortByMyRating(Enums.SortWay sortWay)
        {
            List<QueryResult> queryResults = new List<QueryResult>();
            foreach (var dbid in DbConfigIds)
            {
                var db = Db.GetConnection(dbid);
                if (db != null)
                {
                    if (sortWay == Enums.SortWay.Positive)
                    {
                        var ls = db.Queryable<DbModels.EntryDb>()
                            .OrderBy(p => p.CreateTime)
                            .Select(p => new { p.Id, p.MyRating })
                            .ToList();
                        ls.ForEach(p =>
                        {
                            queryResults.Add(new QueryResult(p.Id, p.MyRating, dbid));
                        });
                    }
                }
            }
            if (sortWay == Enums.SortWay.Positive)
            {
                return queryResults.OrderBy(p => p.Value).ToList();
            }
            else
            {
                return queryResults.OrderByDescending(p => p.Value).ToList();
            }
        }

        public static async Task<Entry> QueryEntryAsync(List<QueryItem> queryItems)
        {

            if(!string.IsNullOrEmpty(id))
            {
                var enrtyDb = await Db.GetConnection(dbId)?.Queryable<EntryDb>().FirstAsync(p=>p.Id == id);
                if(enrtyDb != null)
                {
                    return Entry.Create(enrtyDb, dbId);
                }
            }
            return null;
        }
        public static async Task<List<Entry>> QueryEntryAsync()
        {
            if (!string.IsNullOrEmpty(id))
            {
                var enrtyDb = await Db.GetConnection(dbId)?.Queryable<EntryDb>().FirstAsync(p => p.Id == id);
                if (enrtyDb != null)
                {
                    return Entry.Create(enrtyDb, dbId);
                }
            }
            return null;
        }
    }
}
