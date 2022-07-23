using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    /// <summary>
    /// 每个数据库的LabelDb、EntryLabelDb表数据都统一，必须使用事务提交
    /// 添加、删除标签相关数据时需要每个仓库的表都添加
    /// 查询时查询任意一个均可
    /// </summary>
    public static class LabelService
    {
        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetAllLabelAsync()
        {
            return await Task.Run(()=> DbService.Db.Queryable<LabelDb>().ToList());
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        public static async Task<LabelDb> GetLabel(string labelId)
        {
            return await DbService.Db.Queryable<LabelDb>().FirstAsync(p=>p.Id == labelId);
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<LabelDb> GetLabels(List<string> labelIds)
        {
            return DbService.Db.Queryable<LabelDb>().In(labelIds).ToList();
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetLabelsAsync(List<string> labelIds)
        {
            return await Task.Run(()=> GetLabels(labelIds));
        }
        /// <summary>
        /// 获取标签下所有的词条id
        /// 已去重
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<string> GetEntrys(List<string> labelIds)
        {
            var all = DbService.Db.Queryable<EntryLabelDb>().Where(p => labelIds.Contains(p.LabelId)).ToList().Select(p=>p.EntryId).ToList();
            return all.ToHashSet().ToList();
        }
        /// <summary>
        /// 获取标签下所有的词条id
        /// 已去重
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetLabelOfEntryAsync(string dbid, string entryId)
        {
            return await Task.Run(() =>
            {
                var labelIds = DbService.Db.GetConnection(dbid).Queryable<EntryLabelDb>().Where(p => p.EntryId == entryId).Select(p => p.LabelId).ToList();
                if (labelIds.Count != 0)
                {
                    return GetLabels(labelIds);
                }
                else
                {
                    return null;
                }
            });
        }
        /// <summary>
        /// 清空词条绑定的标签
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static async Task ClearEntryLabelAsync(string entryId)
        {
            //使用事务确保数据统一
            await Task.Run(() =>
            {
                ClearEntryLabel(entryId);
            });
        }
        /// <summary>
        /// 清空词条绑定的标签
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void ClearEntryLabel(string entryId)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Deleteable<EntryLabelDb>(p => p.EntryId == entryId).ExecuteCommand();
            }
            DbService.Db.CommitTran();
        }
        public static async Task AddEntryLabelAsync(List<EntryLabelDb> entryLabeles)
        {
            //使用事务确保数据统一
            await Task.Run(() =>
            {
                AddEntryLabel(entryLabeles);
            });
        }
        public static void AddEntryLabel(List<EntryLabelDb> entryLabeles)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Insertable(entryLabeles).ExecuteCommand();
            }
            DbService.Db.CommitTran();
        }
        public static async Task AddEntryLabelAsyn(EntryLabelDb entryLabel)
        {
            await Task.Run(() =>
            {
                //使用事务确保数据统一
                DbService.Db.BeginTran();
                foreach (var id in DbService.DbConfigIds)
                {
                    DbService.Db.GetConnection(id).Insertable(entryLabel).ExecuteCommand();
                }
                DbService.Db.CommitTran();
            });
        }
        public static void AddLabel(LabelDb labelDb)
        {
            if(string.IsNullOrEmpty(labelDb.Id))
            {
                labelDb.Id = Guid.NewGuid().ToString(); 
            }
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Insertable(labelDb).ExecuteCommand();
            }
            DbService.Db.CommitTran();
        }
        public static void RemoveLabel(string labelId)
        {
            RemoveLabel(new List<string>() { labelId });
        }
        public static void RemoveLabel(List<string> labelIds)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Deleteable<LabelDb>().In(labelIds).ExecuteCommand();
                DbService.Db.GetConnection(id).Deleteable<EntryLabelDb>().Where(p=> labelIds.Contains(p.LabelId));//EntryLabelDb表是没有主键的，不能用in
            }
            DbService.Db.CommitTran();
        }

        public static void UpdateLabel(LabelDb labelDb)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Updateable(labelDb).ExecuteCommand();
            }
            DbService.Db.CommitTran();
        }
    }
}
