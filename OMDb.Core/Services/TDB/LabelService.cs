using OMDb.Core.DbModels;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class LabelService
    {
        private static bool IsLocalDbValid()
        {
            return DbService.LocalDb != null;
        }

        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetAllLabelAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<LabelDb>().ToListAsync();
            }
            else
            {
                return null;
            }
        }


        public static async Task<List<LabelDb>> GetAllLabelAsync(string currentDb)
        {
            if (IsLocalDbValid()) return await DbService.LocalDb.Queryable<LabelDb>().Where(a => a.DbSourceId == currentDb).ToListAsync();
            else return null;
        }


        public static List<LabelDb> GetAllLabel(string currentDb)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select * from Label where DbSourceId='{0}'", currentDb);
            return DbService.LocalDb.Ado.SqlQuery<LabelDb>(sb.ToString());
        }

        public static async Task<int> GetLabelCountAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<LabelDb>().CountAsync();
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取分类标签
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        public static async Task<LabelDb> GetLabel(string labelId)
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<LabelDb>().FirstAsync(p => p.LCId == labelId);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取分类标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<LabelDb> GetLabels(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                return DbService.LocalDb.Queryable<LabelDb>().In(labelIds).ToList();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取分类标签
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetLabelsAsync(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<LabelDb>().In(labelIds).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取标签下所有的词条id
        /// 已去重
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<string> GetEntrys(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                var all = DbService.LocalDb.Queryable<EntryLabelLKDb>().Where(p => labelIds.Contains(p.LCId)).ToList().Select(p => p.EntryId).ToList();
                return all.ToHashSet().ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取标签下所有的词条id
        /// 已去重
        /// </summary>
        /// <param name="labelIds"></param>
        /// <returns></returns>
        public static List<string> GetEntrys(string labelId)
        {
            if (IsLocalDbValid())
            {
                var all = DbService.LocalDb.Queryable<EntryLabelLKDb>().Where(p => p.LCId == labelId).ToList().Select(p => p.EntryId).ToList();
                return all.ToHashSet().ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取拥有标签的词条id
        /// </summary>
        /// <returns>结果已去重</returns>
        public static List<string> GetAllEntryIds()
        {
            if (IsLocalDbValid())
            {
                var all = DbService.LocalDb.Queryable<EntryLabelLKDb>().GroupBy(p => p.EntryId).ToList();
                return all.Select(p => p.EntryId).ToList();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取词条的标签
        /// </summary>
        /// <param name="dbid"></param>
        /// <param name="entryId"></param>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetLabelOfEntryAsync(string entryId)
        {
            if (IsLocalDbValid())
            {
                var labelIds = await DbService.LocalDb.Queryable<EntryLabelLKDb>().Where(p => p.EntryId == entryId).Select(p => p.LCId).ToListAsync();
                if (labelIds.Count != 0)
                {
                    return await GetLabelsAsync(labelIds);
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
        public static List<string> GetLabelIdsOfEntry(string entryId)
        {
            if (IsLocalDbValid())
            {
                return DbService.LocalDb.Queryable<EntryLabelLKDb>().Where(p => p.EntryId == entryId).Select(p => p.LCId).ToList();
            }
            else
            {
                return null;
            }
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
            DbService.LocalDb.Deleteable<EntryLabelLKDb>(p => p.EntryId == entryId).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsync(List<EntryLabelLKDb> entryLabeles)
        {
            //使用事务确保数据统一
            await Task.Run(() =>
            {
                AddEntryLabel(entryLabeles);
            });
        }
        public static void AddEntryLabel(List<EntryLabelLKDb> entryLabeles)
        {
            DbService.LocalDb.Insertable(entryLabeles).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsyn(EntryLabelLKDb entryLabel)
        {
            await Task.Run(() =>
            {
                DbService.LocalDb.Insertable(entryLabel).ExecuteCommand();
            });
        }
        public static void AddLabel(LabelDb labelDb)
        {
            if (string.IsNullOrEmpty(labelDb.LCId))
            {
                labelDb.LCId = Guid.NewGuid().ToString();
            }
            DbService.LocalDb.Insertable(labelDb).ExecuteCommand();
        }

        public static void RemoveLabel(string labelId)
        {
            RemoveLabel(new List<string>() { labelId });
        }
        public static void RemoveLabel(List<string> labelIds)
        {
            DbService.LocalDb.Deleteable<LabelDb>().In(labelIds).ExecuteCommand();
            //清空关联的子分类
            //DbService.LocalDb.Updateable<LabelDb>().SetColumns(p => p.ParentId == null).Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.LocalDb.Deleteable<LabelDb>().Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.LocalDb.Deleteable<EntryLabelLKDb>().Where(p => labelIds.Contains(p.LCId));//EntryLabelLKDb表是没有主键的，不能用in
        }

        public static void UpdateLabel(LabelDb labelDb)
        {
            DbService.LocalDb.Updateable(labelDb).ExecuteCommand();
        }
    }
}
