using OMDb.Core.DbModels;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class LabelPropertyService
    {
        private static bool IsLocalDbValid()
        {
            return DbService.DCDb != null;
        }

        /// <summary>
        /// 获取全部属性标签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<LabelPropertyDb>> GetAllLabelAsync()
        {
            if (IsLocalDbValid())
                return await DbService.DCDb.Queryable<LabelPropertyDb>().ToListAsync();
            else
                return null;
        }


        /// <summary>
        /// 获取当前媒体库所有属性标签
        /// </summary>
        /// <param name="currentDb"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static List<LabelPropertyDb> GetAllLabel(string currentDb)
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelPropertyDb>().Where(a => a.DbCenterId == currentDb).ToList();
            else return null;
        }
        public static async Task<List<LabelPropertyDb>> GetAllLabelAsync(string currentDb)//异步版本
        {
            if (IsLocalDbValid())
                return await DbService.DCDb.Queryable<LabelPropertyDb>().Where(a => a.DbCenterId == currentDb).ToListAsync();
            else return null;
        }



        /// <summary>
        /// 获取属性标签数量
        /// </summary>
        /// <returns></returns>
        public static int GetLabelCount()
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelPropertyDb>().Count();
            else
                return 0;
        }
        public static async Task<int> GetLabelCountAsync()//异步版本
        {
            if (IsLocalDbValid())
                return await DbService.DCDb.Queryable<LabelPropertyDb>().CountAsync();
            else
                return 0;
        }

        /// <summary>
        /// 根据内码获取属性标签
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        public static LabelPropertyDb GetLabels(string labelId)
        {
            return GetLabels(new List<string>() { labelId })[0];
        }
        /// <summary>
        /// 根据内码获取属性标签
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        public static List<LabelPropertyDb> GetLabels(List<string> labelIds)
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelPropertyDb>().In(labelIds).ToList();
            else
                return null;
        }
        public static async Task<List<LabelPropertyDb>> GetLabelsAsync(List<string> labelIds)//异步版本
        {
            if (IsLocalDbValid())
                return await DbService.DCDb.Queryable<LabelPropertyDb>().In(labelIds).ToListAsync();
            else
                return null;
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
                var all = DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => labelIds.Contains(p.LPId)).ToList().Select(p => p.EntryId).ToList();
                return all.ToHashSet().ToList();
            }
            else
                return null;
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
                var all = DbService.DCDb.Queryable<EntryLabelClassLinkDb>().Where(p => p.LCId == labelId).ToList().Select(p => p.EntryId).ToList();
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
                var all = DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().GroupBy(p => p.EntryId).ToList();
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
        public static async Task<List<LabelPropertyDb>> GetLabelOfEntryAsync(string entryId)
        {
            if (IsLocalDbValid())
            {
                var labelIds = await DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LPId).ToListAsync();
                if (labelIds.Count != 0) return await GetLabelsAsync(labelIds);
                else return null;
            }
            else
                return null;
        }

        public static List<string> GetLabelIdsOfEntry(string entryId)
        {
            if (IsLocalDbValid())
            {
                return DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LPId).ToList();
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
            DbService.DCDb.Deleteable<EntryLabelPropertyLinkDb>(p => p.EntryId == entryId).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsync(List<EntryLabelPropertyLinkDb> entryLabeles)
        {
            //使用事务确保数据统一
            await Task.Run(() =>
            {
                AddEntryLabel(entryLabeles);
            });
        }
        public static void AddEntryLabel(List<EntryLabelPropertyLinkDb> entryLabeles)
        {
            DbService.DCDb.Insertable(entryLabeles).ExecuteCommand();
        }
        public static async Task AddEntryLabelAsyn(EntryLabelPropertyLinkDb entryLabel)
        {
            await Task.Run(() =>
            {
                DbService.DCDb.Insertable(entryLabel).ExecuteCommand();
            });
        }
        public static void AddLabel(LabelPropertyDb labelDb)
        {
            if (string.IsNullOrEmpty(labelDb.LPId))
            {
                labelDb.LPId = Guid.NewGuid().ToString();
            }
            DbService.DCDb.Insertable(labelDb).ExecuteCommand();
        }




        public static void RemoveLabel(string labelId)
        {
            RemoveLabel(new List<string>() { labelId });
        }
        public static void RemoveLabel(List<string> lpids)
        {
            DbService.DCDb.Deleteable<LabelPropertyDb>().In(lpids).ExecuteCommand();
            //清空关联的子分类
            //DbService.LocalDb.Updateable<LabelDb>().SetColumns(p => p.ParentId == null).Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<LabelPropertyDb>().Where(p => lpids.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<EntryLabelPropertyLinkDb>().Where(p => lpids.Contains(p.LPId));//EntryLabelDb表是没有主键的，不能用in
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>().Where(p => lpids.Contains(p.LPIdA));
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>().Where(p => lpids.Contains(p.LPIdB));
        }

        public static void UpdateLabel(LabelPropertyDb labelDb)
        {
            DbService.DCDb.Updateable(labelDb).ExecuteCommand();
        }

        public static void AddLabelPropertyLK(string DbCenterId, string lpid, List<string> lst)
        {
            List<LabelPropertyLinkDb> lst_lplk = new List<LabelPropertyLinkDb>();
            foreach (var item in lst)
            {
                lst_lplk.Add(new LabelPropertyLinkDb()
                {
                    DbCenterId = DbCenterId,
                    LPIdA = lpid,
                    LPIdB = item
                });

            }
            DbService.DCDb.Insertable(lst_lplk).ExecuteCommand();
        }

        public static List<string> GetLKId(string DbCenterId, string lpid)
        {
            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            sb1.AppendLine($@"select LPIdA from LabelPropertyLKDb where LPIdB='{lpid}' and DbCenterId='{DbCenterId}'");
            sb2.AppendLine($@"select LPIdB from LabelPropertyLKDb where LPIdA='{lpid}' and DbCenterId='{DbCenterId}'");
            var lst1 = DbService.DCDb.Ado.SqlQuery<string>(sb1.ToString());
            var lst2 = DbService.DCDb.Ado.SqlQuery<string>(sb2.ToString());
            lst1.AddRange(lst2);
            var lst=lst1.Distinct();
            return lst.ToList();
        }

        /// <summary>
        /// 清空词条绑定的标签
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void ClearLabelPropertyLK(string DbCenterId, string lpid)
        {
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>(p => p.LPIdA == lpid||p.LPIdB==lpid).ExecuteCommand();
        }
    }
}
