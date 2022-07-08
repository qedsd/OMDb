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
        public static async Task<LabelDb> GetLabel(int labelId)
        {
            return await DbService.Db.Queryable<LabelDb>().FirstAsync(p=>p.Id == labelId);
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<LabelDb> GetLabels(List<int> labelIds)
        {
            return DbService.Db.Queryable<LabelDb>().In(labelIds).ToList();
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetLabelsAsync(List<int> labelIds)
        {
            return await Task.Run(()=> GetLabels(labelIds));
        }
        /// <summary>
        /// 获取标签下所有的词条id
        /// 已去重
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<string> GetEntrys(List<int> labelIds)
        {
            var all = DbService.Db.Queryable<EntryLabelDb>().Where(p => labelIds.Contains(p.LabelId)).ToList().Select(p=>p.EntryId).ToList();
            return all.ToHashSet().ToList();
        }

        public static void AddLabel(LabelDb labelDb)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Insertable(labelDb);
            }
            DbService.Db.CommitTran();
        }
        public static void RemoveLabel(List<int> labelIds)
        {
            //使用事务确保数据统一
            DbService.Db.BeginTran();
            foreach (var id in DbService.DbConfigIds)
            {
                DbService.Db.GetConnection(id).Deleteable<LabelDb>().In(labelIds);
                DbService.Db.GetConnection(id).Deleteable<EntryLabelDb>().Where(p=> labelIds.Contains(p.LabelId));//EntryLabelDb表是没有主键的，不能用in
            }
            DbService.Db.CommitTran();
        }
    }
}
