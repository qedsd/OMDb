using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    /// <summary>
    /// 前提条件是每个数据库的LabelDb、EntryLabelDb表数据都统一
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
