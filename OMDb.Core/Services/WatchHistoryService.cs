using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class WatchHistoryService
    {
        /// <summary>
        /// 查询词条所有观看记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task<List<WatchHistory>> QueryWatchHistoriesAsync(string id, string dbId)
        {
            var entryNameDbs = await Task.Run(() => DbService.GetConnection(dbId).Queryable<WatchHistoryDb>().Where(p => p.Id == id).ToList());
            if (entryNameDbs.Any())
            {
                return entryNameDbs.Select(p => WatchHistory.Create(p, dbId)).ToList();
            }
            else
            {
                return null;
            }
        }

        public static void AddWatchHistory(Models.WatchHistory watchHistory)
        {
            DbService.GetConnection(watchHistory.DbId).Insertable(watchHistory as DbModels.WatchHistoryDb).ExecuteCommand();
        }

        public static void UpdateWatchHistory(Models.WatchHistory watchHistory)
        {
            DbService.GetConnection(watchHistory.DbId).Updateable(watchHistory as DbModels.WatchHistoryDb).ExecuteCommand();
        }
        public static void DeleteWatchHistory(Models.WatchHistory watchHistory)
        {
            DbService.GetConnection(watchHistory.DbId).Deleteable(watchHistory as DbModels.WatchHistoryDb).ExecuteCommand();
        }
    }
}
