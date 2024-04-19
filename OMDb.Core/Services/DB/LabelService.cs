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
            return DbService.ConfigDb != null;
        }

        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LabelDb>> GetAllLabelAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.ConfigDb.Queryable<LabelDb>().ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public static async Task<int> GetLabelCountAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.ConfigDb.Queryable<LabelDb>().CountAsync();
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
                return await DbService.ConfigDb.Queryable<LabelDb>().FirstAsync(p => p.ID == labelId);
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
                return DbService.ConfigDb.Queryable<LabelDb>().In(labelIds).ToList();
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
                return await DbService.ConfigDb.Queryable<LabelDb>().In(labelIds).ToListAsync();
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
                var all = DbService.ConfigDb.Queryable<EntryLabelLinkDb>().Where(p => labelIds.Contains(p.LabelID)).ToList().Select(p => p.EntryId).ToList();
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
                var all = DbService.ConfigDb.Queryable<EntryLabelLinkDb>().Where(p => p.LabelID == labelId).ToList().Select(p => p.EntryId).ToList();
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
                var all = DbService.ConfigDb.Queryable<EntryLabelLinkDb>().GroupBy(p => p.EntryId).ToList();
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
                var labelIds = await DbService.ConfigDb.Queryable<EntryLabelLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LabelID).ToListAsync();
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
                return DbService.ConfigDb.Queryable<EntryLabelLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LabelID).ToList();
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
            await DbService.ConfigDb.Deleteable<EntryLabelLinkDb>(p => p.EntryId == entryId).ExecuteCommandAsync();
        }
        /// <summary>
        /// 清空词条绑定的标签
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void ClearEntryLabel(string entryId)
        {
            DbService.ConfigDb.Deleteable<EntryLabelLinkDb>(p => p.EntryId == entryId).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsync(List<EntryLabelLinkDb> entryLabeles)
        {
            await DbService.ConfigDb.Insertable(entryLabeles).ExecuteCommandAsync();
        }
        public static void AddEntryLabel(List<EntryLabelLinkDb> entryLabeles)
        {
            DbService.ConfigDb.Insertable(entryLabeles).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsyn(EntryLabelLinkDb entryLabel)
        {
            await DbService.ConfigDb.Insertable(entryLabel).ExecuteCommandAsync();
        }
        public static void AddLabel(LabelDb labelDb)
        {
            if (string.IsNullOrEmpty(labelDb.ID))
            {
                labelDb.ID = Guid.NewGuid().ToString();
            }
            DbService.ConfigDb.Insertable(labelDb).ExecuteCommand();
        }

        public static void RemoveLabel(string labelId)
        {
            RemoveLabel(new List<string>() { labelId });
        }
        public static void RemoveLabel(List<string> labelIds)
        {
            DbService.ConfigDb.Deleteable<LabelDb>().In(labelIds).ExecuteCommand();
            DbService.ConfigDb.Deleteable<EntryLabelLinkDb>().Where(p => labelIds.Contains(p.LabelID));
        }

        public static void UpdateLabel(LabelDb labelDb)
        {
            DbService.ConfigDb.Updateable(labelDb).ExecuteCommand();
        }
    }
}
