using NPOI.Util;
using OMDb.Core.DbModels;
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
        public static List<LabelPropertyDb> GetAllLabelProperty(string currentDb)
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelPropertyDb>().Where(a => a.DbCenterId == currentDb).ToList();
            else return null;
        }
        public static async Task<List<LabelPropertyDb>> GetAllLabelPropertyAsync(string currentDb)//异步版本
        {
            if (IsLocalDbValid())
                return await DbService.DCDb.Queryable<LabelPropertyDb>().Where(a => a.DbCenterId == currentDb).ToListAsync();
            else return null;
        }

        public static List<LabelPropertyDb> GetLabelPropertyHeader()
        {
            if (IsLocalDbValid())
                return DbService.DCDb.Queryable<LabelPropertyDb>().Where(a => a.Level == 1).ToList();
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
        /// <returns></returns>using System.Linq;
        public static List<string> GetEntrys(List<string> labelIds)
        {
            if (IsLocalDbValid())
            {
                var all = DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => labelIds.Contains(p.LPID)).ToList().Select(p => p.EntryId).ToList();
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
                var labelIds = await DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LPID).ToListAsync();
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
                return DbService.DCDb.Queryable<EntryLabelPropertyLinkDb>().Where(p => p.EntryId == entryId).Select(p => p.LPID).ToList();
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

        public static LabelPropertyDb AddLabelProperty(LabelPropertyDb labelPropertyDb)
        {
            if (labelPropertyDb.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                throw new Exception("[标签名]必录");

            if (string.IsNullOrEmpty(labelPropertyDb.LPID))
                labelPropertyDb.LPID = Guid.NewGuid().ToString();

            DbService.DCDb.Insertable(labelPropertyDb).ExecuteCommand();
            return labelPropertyDb;
        }

        public static void AddLabelProperty(List<LabelPropertyDb> labelPropertyDbList)
        {
            var labelPropertyDbListWithName=new List<LabelPropertyDb>();
            foreach (var labelPropertyDb in labelPropertyDbList)
            {
                if (!labelPropertyDb.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                {
                    if (string.IsNullOrEmpty(labelPropertyDb.LPID))
                        labelPropertyDb.LPID = Guid.NewGuid().ToString();
                    labelPropertyDbListWithName.Add(labelPropertyDb);
                }
            }
            DbService.DCDb.Insertable<LabelPropertyDb>(labelPropertyDbListWithName).ExecuteCommand();
        }


        public static void RemoveLabel(string labelPropertyId)
        {
            RemoveLabel(new List<string>() { labelPropertyId });
        }
        public static void RemoveLabel(List<string> labelPropertyIdList)
        {
            DbService.DCDb.Deleteable<LabelPropertyDb>().In(labelPropertyIdList).ExecuteCommand();
            //清空关联的子分类
            //DbService.LocalDb.Updateable<LabelDb>().SetColumns(p => p.ParentId == null).Where(p => labelIds.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<LabelPropertyDb>().Where(p => labelPropertyIdList.Contains(p.ParentId)).ExecuteCommand();
            DbService.DCDb.Deleteable<EntryLabelPropertyLinkDb>().Where(p => labelPropertyIdList.Contains(p.LPID));//EntryLabelDb表是没有主键的，不能用in
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>().Where(p => labelPropertyIdList.Contains(p.LPIDA));
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>().Where(p => labelPropertyIdList.Contains(p.LPIDB));
        }

        public static void UpdateLabel(LabelPropertyDb labelDb)
        {
            DbService.DCDb.Updateable(labelDb).ExecuteCommand();
        }

        public static void AddLabelPropertyLink(string lablePropertyId, List<string> lablePropertyIdList)
        {
            List<LabelPropertyLinkDb> labelPropertyLinkDbList = new List<LabelPropertyLinkDb>();
            foreach (var item in lablePropertyIdList)
            {
                labelPropertyLinkDbList.Add(new LabelPropertyLinkDb()
                {
                    LPIDA = lablePropertyId,
                    LPIDB = item
                });

            }
            DbService.DCDb.Insertable(labelPropertyLinkDbList).ExecuteCommand();
        }

        public static List<string> GetLinkIdList(string lablePropertyId)
        {
            var sql = $@"
                            SELECT LPIDA
                            FROM   LabelPropertyLink
                            WHERE  LPIDB = '{lablePropertyId}'
                            UNION ALL
                            SELECT LPIDB
                            FROM   LabelPropertyLink
                            WHERE  LPIDA = '{lablePropertyId}' 
                        ";
            var linkIdList = DbService.DCDb.Ado.SqlQuery<string>(sql);
            return linkIdList;
        }

        /// <summary>
        /// 清空词条绑定的标签
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void ClearLabelPropertyLink(string lablePropertyId)
        {
            DbService.DCDb.Deleteable<LabelPropertyLinkDb>(p => p.LPIDA == lablePropertyId || p.LPIDB == lablePropertyId).ExecuteCommand();
        }

        /// <summary>
        /// 删除Link
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void RemoveLabelPropertyLink(string lablePropertyId, List<string> linkIds)
        {
            foreach (var linkId in linkIds)
            {
                var conditionlablePropertyDataStr = @$"SELECT LPID FROM LabelProperty WHERE ParentId = '{lablePropertyId}'";
                var conditionLinklablePropertyDataStr = @$"SELECT LPID FROM LabelProperty WHERE ParentId = '{linkId}'";
                var sql = $@" DELETE
                              FROM  LabelPropertyLink
                              WHERE ( LPIDA IN ({conditionlablePropertyDataStr}) AND LPIDB IN ({conditionLinklablePropertyDataStr}) )
                              OR    ( LPIDA IN ({conditionLinklablePropertyDataStr}) AND LPIDB IN ({conditionlablePropertyDataStr}) )
                              OR    ( LPIDA = ('{linkId}') AND LPIDB='{lablePropertyId}' )
                              OR    ( LPIDB = ('{lablePropertyId}') AND LPIDB='{linkId}' )
                        ";
                DbService.DCDb.Ado.SqlQuery<string>(sql);
            }

        }
    }
}