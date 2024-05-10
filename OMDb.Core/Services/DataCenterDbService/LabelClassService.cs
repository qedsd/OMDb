using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class LabelClassService
    {
        private static bool IsLocalDbValid()
        {
            return DbService.DCDb != null;
        }

        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LabelClassDb>> GetAllLabelAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<LabelClassDb>().ToListAsync();
            }
            else
            {
                return null;
            }
        }


        public static async Task<List<LabelClassDb>> GetAllLabelAsync(string currentDb)
        {
            if (IsLocalDbValid()) return await DbService.DCDb.Queryable<LabelClassDb>().Where(a => a.DbCenterId == currentDb).ToListAsync();
            else return null;
        }


        public static List<LabelClassDb> GetAllLabel(string currentDb)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select * from LabelClass where DbCenterId='{0}'", currentDb);
            return DbService.DCDb.Ado.SqlQuery<LabelClassDb>(sb.ToString());
        }

        public static async Task<int> GetLabelCountAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<LabelClassDb>().CountAsync();
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
        public static async Task<LabelClassDb> GetLabel(string labelId)
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<LabelClassDb>().FirstAsync(p => p.LCID == labelId);
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
        public static List<LabelClassDb> GetLabels(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                return DbService.DCDb.Queryable<LabelClassDb>().In(labelIds).ToList();
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
        public static async Task<List<LabelClassDb>> GetLabelsAsync(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<LabelClassDb>().In(labelIds).ToListAsync();
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
                var all = DbService.DCDb.Queryable<EntryLabelClassLinkDb>().Where(p => labelIds.Contains(p.LCID)).ToList().Select(p => p.EntryId).ToList();
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
                var all = DbService.DCDb.Queryable<EntryLabelClassLinkDb>().Where(p => p.LCID == labelId).ToList().Select(p => p.EntryId).ToList();
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
                var all = DbService.DCDb.Queryable<EntryLabelClassLinkDb>().GroupBy(p => p.EntryId).ToList();
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
        public static async Task<List<LabelClassDb>> GetLabelOfEntryAsync(string entryId)
        {
            if (IsLocalDbValid())
            {
                var labelIds = await DbService.DCDb.Queryable<EntryLabelClassLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LCID).ToListAsync();
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
                return DbService.DCDb.Queryable<EntryLabelClassLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LCID).ToList();
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
            DbService.DCDb.Deleteable<EntryLabelClassLinkDb>(p => p.EntryId == entryId).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsync(List<EntryLabelClassLinkDb> entryLabeles)
        {
            //使用事务确保数据统一
            await Task.Run(() =>
            {
                AddEntryLabel(entryLabeles);
            });
        }
        public static void AddEntryLabel(List<EntryLabelClassLinkDb> entryLabeles)
        {
            DbService.DCDb.Insertable(entryLabeles).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsyn(EntryLabelClassLinkDb entryLabel)
        {
            await Task.Run(() =>
            {
                DbService.DCDb.Insertable(entryLabel).ExecuteCommand();
            });
        }
        public static void AddLabelClass(LabelClassDb labelClassDb)
        {
            if (labelClassDb.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                return;
            if (string.IsNullOrEmpty(labelClassDb.LCID))
                labelClassDb.LCID = Guid.NewGuid().ToString();
            
            DbService.DCDb.Insertable(labelClassDb).ExecuteCommand();
        }

        public static void AddLabelClass(List<LabelClassDb> labelClassDbList)
        {
            var labelClassDbListWithName = new List<LabelClassDb>();
            foreach (var labelClassDb in labelClassDbList)
            {
                if (!labelClassDb.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                {
                    if (string.IsNullOrEmpty(labelClassDb.LCID))
                        labelClassDb.LCID = Guid.NewGuid().ToString();
                    labelClassDbListWithName.Add(labelClassDb);
                }
            }
            DbService.DCDb.Insertable<LabelClassDb>(labelClassDbListWithName).ExecuteCommand();
        }

        public static void RemoveLabelClass(string labelId)
        {
            RemoveLabelClass(new List<string>() { labelId });
        }
        public static void RemoveLabelClass(List<string> labelIds)
        {
            DbService.DCDb.Deleteable<LabelClassDb>().In(labelIds).ExecuteCommand();
            //清空关联的子分类
            //DbService.LocalDb.Updateable<LabelDb>().SetColumns(p => p.ParentId == null).Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<LabelClassDb>().Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<EntryLabelClassLinkDb>().Where(p => labelIds.Contains(p.LCID));//EntryLabelLKDb表是没有主键的，不能用in
        }

        public static void UpdateLabelClass(LabelClassDb labelDb)
        {
            DbService.DCDb.Updateable(labelDb).ExecuteCommand();
        }

        //获取一级分类标签
        public static List<LabelClassDb> GetLabelClassL1()
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelClassDb>().Where(a => a.Level == 1).ToList();
            else return null;
        }
    }
}
