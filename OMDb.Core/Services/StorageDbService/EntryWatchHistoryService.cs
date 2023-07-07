using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryWatchHistoryService
    {
        /// <summary>
        /// 查询词条所有观看记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task<List<WatchHistory>> QueryWatchHistoriesAsync(string id, string dbId)
        {
            var entryNameDbs = await Task.Run(() => DbService.GetConnection(dbId).Queryable<EntryWatchHistoryDb>().Where(p => p.EntryId == id).ToList());
            if (entryNameDbs.Any())
            {
                return entryNameDbs.Select(p => WatchHistory.Create(p, dbId)).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询词条所有观看记录
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task<List<WatchHistory>> QueryWatchHistoriesAsync(List<string> ids, string dbId)
        {
            var entryNameDbs = await DbService.GetConnection(dbId).Queryable<EntryWatchHistoryDb>().Where(p => ids.Contains(p.EntryId)).ToListAsync();
            if (entryNameDbs.Any())
            {
                return entryNameDbs.Select(p => WatchHistory.Create(p, dbId)).ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加观看历史
        /// 会同步更新词条的观看次数
        /// </summary>
        /// <param name="watchHistory"></param>
        public static bool AddWatchHistory(Models.WatchHistory watchHistory)
        {
            if(DbService.GetConnection(watchHistory.DbId).Insertable(watchHistory as DbModels.EntryWatchHistoryDb).ExecuteCommand() > 0)
            {
                if (watchHistory.Done)
                {
                    return EntryService.UpdateWatchTime(watchHistory.EntryId, watchHistory.DbId, 1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新观看记录
        /// 会同步更新词条的观看次数
        /// </summary>
        /// <param name="watchHistory"></param>
        /// <returns></returns>
        public static bool UpdateWatchHistory(Models.WatchHistory watchHistory)
        {
            //需要判断是否修改了观看完成
            var source = DbService.GetConnection(watchHistory.DbId).Queryable<EntryWatchHistoryDb>().In(watchHistory.Id).First();
            if(source != null)
            {
                var update = DbService.GetConnection(watchHistory.DbId).Updateable(watchHistory as DbModels.EntryWatchHistoryDb).ExecuteCommand() > 0;
                if(!update)
                {
                    return false;
                }
                else
                {
                    if (watchHistory.Done != source.Done)
                    {
                        return EntryService.UpdateWatchTime(watchHistory.EntryId, watchHistory.DbId, source.Done ? 1 : -1);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 删除观看记录
        /// 会同步更新词条的观看次数
        /// </summary>
        /// <param name="watchHistory"></param>
        /// <returns></returns>
        public static bool DeleteWatchHistory(Models.WatchHistory watchHistory)
        {
            if(DbService.GetConnection(watchHistory.DbId).Deleteable(watchHistory as DbModels.EntryWatchHistoryDb).ExecuteCommand() > 0)
            {
                if (watchHistory.Done)
                {
                    return EntryService.UpdateWatchTime(watchHistory.EntryId, watchHistory.DbId, -1);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
