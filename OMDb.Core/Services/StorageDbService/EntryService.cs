using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryService
    {
        /// <summary>
        /// 查询词条(异步)
        /// </summary>
        /// <param name="sortModel"></param>
        /// <param name="filterModel"></param>
        /// <returns></returns>
        public static async Task<List<QueryResult>> QueryEntryAsync(SortModel sortModel, FilterModel filterModel)
        {
            return await Task.Run(() => QueryEntry(sortModel, filterModel));
        }

        /// <summary>
        /// 按指定排序方式获取词条
        /// 获取完自行处理分页
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="sortWay"></param>
        /// <returns></returns>
        public static List<QueryResult> QueryEntry(SortModel sortModel, FilterModel filterModel)
        {
            if (DbService.IsEmpty)
                return null;

            List<QueryResult> queryResults = new List<QueryResult>();

            var inLabelClassEntryIds = new List<string>();
            if (filterModel.LabelClassIds != null && filterModel.LabelClassIds.Count != 0)
                inLabelClassEntryIds = LabelClassService.GetEntrys(filterModel.LabelClassIds);

            var inLabelPropertyEntryIds = new List<string>();
            if (filterModel.LabelPropertyIds != null && filterModel.LabelPropertyIds.Count != 0)
                inLabelPropertyEntryIds = LabelPropertyService.GetEntrys(filterModel.LabelPropertyIds);

            var inLabelEntryIds = new List<string>();
            inLabelEntryIds.AddRange(inLabelClassEntryIds);
            inLabelEntryIds.AddRange(inLabelPropertyEntryIds);

            foreach (var item in DbService.Dbs)
            {
                if (filterModel.IsFilterStorage && !filterModel.StorageIds.Contains(item.Key))
                    continue;

                var db = item.Value;

                //动态拼表达式查询
                var exp = Expressionable.Create<DbModels.EntryDb>()
                .And(a => a.ReleaseDate >= filterModel.BusinessDateBegin)
                .And(a => a.ReleaseDate <= filterModel.BusinessDateEnd)
                .And(a => a.CreateTime >= filterModel.CreateDateBegin)
                .And(a => a.CreateTime <= filterModel.CreateDateEnd)
                .And(a => a.MyRating >= filterModel.RateMin)
                .And(a => a.MyRating <= filterModel.RateMax)
                .ToExpression();


                var ls = db.Queryable<DbModels.EntryDb>().Where(exp);

                if (filterModel.IsFilterLabelProperty)
                    ls = ls.In(inLabelPropertyEntryIds);
                if (filterModel.IsFilterLabelClass)
                    ls = ls.In(inLabelClassEntryIds);

                var result = ls.ToList();
                result.ForEach(p => { queryResults.Add(new QueryResult(p.EntryId, p, item.Key)); });

            }
            if (sortModel.SortWay == Enums.SortWay.Positive)//正序
            {
                return sortModel.SortType switch
                {
                    Enums.SortType.MyRating => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).MyRating).ToList(),
                    Enums.SortType.BusinessDate => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).ReleaseDate).ToList(),
                    Enums.SortType.CreateTime => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).CreateTime).ToList(),
                    Enums.SortType.WatchTimes => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).WatchTimes).ToList(),
                    Enums.SortType.LastWatchTime => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).LastWatchTime).ToList(),
                    Enums.SortType.LastUpdateTime => queryResults = queryResults.OrderBy(p => (p.Value as EntryDb).LastUpdateTime).ToList(),
                };
            }
            else//倒叙
            {
                return sortModel.SortType switch
                {
                    Enums.SortType.MyRating => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).MyRating).ToList(),
                    Enums.SortType.BusinessDate => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).ReleaseDate).ToList(),
                    Enums.SortType.CreateTime => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).CreateTime).ToList(),
                    Enums.SortType.WatchTimes => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).WatchTimes).ToList(),
                    Enums.SortType.LastWatchTime => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).LastWatchTime).ToList(),
                    Enums.SortType.LastUpdateTime => queryResults = queryResults.OrderByDescending(p => (p.Value as EntryDb).LastUpdateTime).ToList(),
                };
            }
        }




        /// <summary>
        /// 查询词条
        /// </summary>
        /// <param name="queryItems"></param>
        /// <param name="withName">结果是否带默认名字</param>
        /// <returns></returns>
        public static async Task<List<Entry>> QueryEntryAsync(List<QueryItem> queryItems, bool withName = true)
        {
            if (DbService.IsEmpty)
            {
                return null;
            }
            if (queryItems != null)
            {
                return await Task.Run(() =>
                {
                    Dictionary<string, Entry> dic = new Dictionary<string, Entry>();
                    var group = queryItems.GroupBy(p => p.DbId);
                    foreach (var item in group)
                    {
                        var entryDbs = DbService.GetConnection(item.Key).Queryable<DbModels.EntryDb>().Where(p2 => item.Select(p => p.Id).Contains(p2.EntryId)).ToList();
                        if (entryDbs.Any())
                        {
                            var entriesTemp = entryDbs.Select(p => Entry.Create(p, item.Key)).ToList();
                            if (withName)
                            {
                                EntryNameSerivce.SetName(entriesTemp, item.Key);
                            }
                            //entries.AddRange(entriesTemp);
                            entriesTemp.ForEach(p =>
                            {
                                dic.Add(p.EntryId, p);
                            });
                        }
                    }
                    List<Entry> entries = new List<Entry>();
                    foreach (var item in queryItems)
                    {
                        if (dic.TryGetValue(item.Id, out Entry entry))
                        {
                            entries.Add(entry);
                        }
                    }
                    return entries;
                });
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 查询词条
        /// </summary>
        /// <param name="queryItem"></param>
        /// <param name="withName">结果是否带默认名字</param>
        /// <returns></returns>
        public static async Task<Entry> QueryEntryAsync(QueryItem queryItem, bool withName = true)
        {
            if (DbService.IsEmpty)
            {
                return null;
            }
            if (queryItem != null)
            {
                var result = await DbService.GetConnection(queryItem.DbId).Queryable<DbModels.EntryDb>().FirstAsync(p => p.EntryId == queryItem.Id);
                if (result != null)
                {
                    var entry = Entry.Create(result, queryItem.DbId);
                    if (withName)
                    {
                        entry.Name = EntryNameSerivce.QueryName(entry.EntryId, queryItem.DbId);
                    }
                    return entry;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新增或编辑词条主表
        /// </summary>
        /// <param name="entry"></param>
        public static void UpdateOrAddEntry(Entry entry)
        {
            if (string.IsNullOrEmpty(entry.EntryId))
                entry.EntryId = Guid.NewGuid().ToString();
            entry.CoverImg = entry.CoverImg.GetDefaultCoverName();
            var existingEntry = DbService.GetConnection(entry.DbId).Queryable<EntryDb>().Where(s => s.EntryId == entry.EntryId).First();
            if (existingEntry == null)
                DbService.GetConnection(entry.DbId).Insertable(entry as EntryDb).ExecuteCommand();
            else
                DbService.GetConnection(entry.DbId).Updateable<EntryDb>(entry as EntryDb).RemoveDataCache().ExecuteCommand();
        }

        /// <summary>
        /// 从数据库移除词条
        /// 不会删除文件
        /// </summary>
        /// <param name="entry"></param>
        public static void RemoveEntry(Entry entry)
        {
            var connet = DbService.GetConnection(entry.DbId);
            connet.BeginTran();
            connet.Deleteable<EntryDb>().In(entry.EntryId).ExecuteCommand();
            connet.Deleteable<EntryNameDb>().Where(p => p.EntryId == entry.EntryId).ExecuteCommand();
            connet.Deleteable<EntryWatchHistoryDb>().Where(p => p.EntryId == entry.EntryId).ExecuteCommand();
            DbService.DCDb.Deleteable<EntryLabelClassLinkDb>().Where(p => p.EntryId == entry.EntryId).ExecuteCommand();
            DbService.DCDb.Deleteable<EntryLabelPropertyLinkDb>().Where(p => p.EntryId == entry.EntryId).ExecuteCommand();
            connet.CommitTran();
        }

        /// <summary>
        /// 从数据库移除词条
        /// 不会删除文件
        /// </summary>
        /// <param name="entries"></param>
        public static void RemoveEntry(List<Entry> entries)
        {
            entries.GroupBy(p => p.DbId).ToList().ForEach(g =>
              {
                  var ids = g.Select(p => p.EntryId);
                  var connet = DbService.GetConnection(g.Key);
                  connet.BeginTran();
                  connet.Deleteable<EntryDb>().In(ids).ExecuteCommand();
                  connet.Deleteable<EntryNameDb>().In(ids).ExecuteCommand();
                  connet.Deleteable<EntryWatchHistoryDb>().In(ids).ExecuteCommand();
                  DbService.DCDb.Deleteable<EntryLabelClassLinkDb>().Where(p => ids.Contains(p.EntryId)).ExecuteCommand();
                  connet.CommitTran();
              });
        }

        public static async Task<int> QueryEntryCountAsync(string dbId)
        {
            return await DbService.GetConnection(dbId).Queryable<EntryDb>().CountAsync();
        }

        public static async Task<List<Entry>> GetEntryByIdsAsync(IEnumerable<string> entryIds, string dbId)
        {
            return await QueryEntryAsync(entryIds.Select(p => new QueryItem(p, dbId)).ToList());
        }

        /// <summary>
        /// 更新词条观看次数
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <param name="increment">变化量，可用正负来表示增加减少</param>
        /// <returns></returns>
        public static bool UpdateWatchTime(string entryId, string dbId, int increment)
        {
            return DbService.GetConnection(dbId).Updateable<EntryDb>().SetColumns(p => p.WatchTimes == p.WatchTimes + increment).Where(p => p.EntryId == entryId).ExecuteCommand() > 0;
        }

        /// <summary>
        /// 从所有的仓库中随机指定个数词条
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task<List<Entry>> RandomEntryAsync(int count = 1)
        {
            if (DbService.IsEmpty)
            {
                return null;
            }
            List<Tuple<DbModels.EntryDb, string>> firstRandoms = new List<Tuple<DbModels.EntryDb, string>>();
            foreach (var dbId in DbService.Dbs.Keys)
            {
                int max = await QueryEntryCountAsync(dbId);
                var indexes = Helpers.RandomHelper.RandomInt(0, max - 1, count);
                if (indexes.NotNullAndEmpty())
                {
                    foreach (var index in indexes)
                    {
                        var result = await DbService.GetConnection(dbId).Queryable<DbModels.EntryDb>().Skip(index).Take(1).ToListAsync();
                        firstRandoms.Add((Tuple.Create(result.First(), dbId)));
                    }
                }
            }
            var items = Helpers.RandomHelper.RandomInt(0, firstRandoms.Count - 1, count);
            List<Entry> entries = new List<Entry>();
            if (!(items == null))
            {
                foreach (var item in items)
                {
                    var tuple = firstRandoms[item];
                    entries.Add(Models.Entry.Create(tuple.Item1, tuple.Item2));
                }
            }
            return entries;
        }


        public static void AddEntry(EntryDb edb, string dbId)
        {
            DbService.GetConnection(dbId).Insertable(edb).ExecuteCommand();
        }
        public static void UpdateEntry(EntryDb edb, string dbId)
        {
            DbService.GetConnection(dbId).Updateable(edb).Where(a => a.EntryId.Equals(edb.EntryId)).ExecuteCommand();
        }
    }
}
