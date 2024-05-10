using CommunityToolkit.WinUI.UI.Controls;
using NPOI.POIFS.FileSystem;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Utils;
using OMDb.WinUI3.Events;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using OMDb.Core.Services;
using OMDb.Core.Enums;
using System.Transactions;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.Models.EntryModels;

namespace OMDb.WinUI3.Services
{
    public static class EntryService
    {
        /// <summary>
        /// 新增词条
        /// </summary>
        /// <returns>词条唯一id</returns>
        public static async Task AddEntryAsync()
        {
            var entryDetail = await Dialogs.EditEntryDialog.ShowDialog();
            if (entryDetail.IsNullOrEmptyOrWhiteSpazeOrCountZero()) return;
            await NewEntryAsync(entryDetail);
            Helpers.InfoHelper.ShowSuccess("新增词条成功");
        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <returns></returns>
        public static async Task AddEntryBatchAsync()
        {
            var eds = await Dialogs.AddEntryBatchDialog.ShowDialog();
            if (eds.IsNullOrEmptyOrWhiteSpazeOrCountZero()) return;
            foreach (var ed in eds)
                await NewEntryAsync(ed);
            Helpers.InfoHelper.ShowSuccess("批量新增词条成功");
        }
        /// <summary>
        /// 编辑词条
        /// </summary>
        /// <returns>词条唯一id</returns>
        public static async Task EditEntryAsync(Core.Models.Entry entry)
        {
            var entryDetail = await Dialogs.EditEntryDialog.ShowDialog(entry);
            if (entryDetail.IsNullOrEmptyOrWhiteSpazeOrCountZero()) return;
            InitFile(entryDetail);
            await InsertOrUpdateDbAsync(entryDetail);
            Helpers.InfoHelper.ShowSuccess("已保存");
            GlobalEvent.NotifyUpdateEntry(null, new EntryEventArgs(entryDetail.Entry));

        }
        /// <summary>
        /// 移除词条
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task RemoveEntryAsync(Core.Models.Entry entry)
        {
            if (!await Dialogs.QueryDialog.ShowDialog("删除", $"是否删除 {entry.Name} 词条?不会删除本地文件")) return;
            Core.Services.EntryService.RemoveEntry(entry);
            Helpers.InfoHelper.ShowSuccess("已删除");
            GlobalEvent.NotifyRemoveEntry(null, new EntryEventArgs(entry));
        }
        /// <summary>
        /// 新增词条
        /// </summary>
        /// <param name="ed"></param>
        /// <param name="isNew"></param>
        /// <returns></returns>
        public static async Task NewEntryAsync(EntryDetail ed)
        {
            //创建词条根目录
            if (Directory.Exists(ed.FullEntryPath))
            {
                var my_format = "yyyyMMddHHmmss";
                string newPath = $"{ed.FullEntryPath}_{DateTime.Now.ToString(my_format)}";
                ed.FullEntryPath = newPath;
                ed.Entry.Path = newPath;
            }
            InitFolder(ed);//创建词条文件夹
            InitFile(ed);//创建元数据(MataData)
            await InsertOrUpdateDbAsync(ed);//保存至数据库
            GlobalEvent.NotifyAddEntry(null, new EntryEventArgs(ed.Entry));
        }
        /// <summary>
        /// 词条数据保存至数据库
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="entryNames"></param>
        /// <returns></returns>
        private static async Task InsertOrUpdateDbAsync(Models.EntryDetail ed)
        {
            await Task.Run(() =>
            {
                TreatEntry(ed);//处理词条主表Entry
                Core.Services.EntryService.UpdateOrAddEntry(ed.Entry);//更新词条主表

                TreatEntrySource(ed);//资源路径表EntrySource处理及更新

                TreatLabelLink(ed);//词条标签关联表

                TreatEntryName(ed);//词条名称表EntryName
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(ed.Entry.EntryId, ed.Entry.DbId, ed.Entry.Name);//更新或插入词条默认名称
            });
        }
        /// <summary>
        /// 标签词条关联表处理
        /// </summary>
        /// <param name="ed"></param>
        private static void TreatLabelLink(EntryDetail ed)
        {
            if (!ed.LabelClassDbList.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                List<Core.DbModels.EntryLabelClassLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelClassLinkDb>(ed.LabelClassDbList.Count);
                ed.LabelClassDbList.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelClassLinkDb() { EntryId = ed.Entry.EntryId, LCID = p.LCID, DbId = ed.Entry.DbId }));
                Core.Services.LabelClassService.ClearEntryLabel(ed.Entry.EntryId);//清空词条分类标签
                Core.Services.LabelClassService.AddEntryLabel(entryLabelDbs);//添加词条分类标签
            }
            if (!ed.LablePropertyDbList.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                List<Core.DbModels.EntryLabelPropertyLinkDb> entryLabelPropertyDbs = new List<Core.DbModels.EntryLabelPropertyLinkDb>(ed.LablePropertyDbList.Count);
                ed.LablePropertyDbList.ForEach(p => entryLabelPropertyDbs.Add(new Core.DbModels.EntryLabelPropertyLinkDb() { EntryId = ed.Entry.EntryId, LPID = p.LPID, DbId = ed.Entry.DbId }));
                Core.Services.LabelPropertyService.ClearEntryLabel(ed.Entry.EntryId);//清空词条属性标签
                Core.Services.LabelPropertyService.AddEntryLabel(entryLabelPropertyDbs);//添加词条属性标签
            }
        }
        /// <summary>
        /// 表EntrySource数据处理
        /// </summary>
        /// <param name="ed"></param>
        private static void TreatEntrySource(EntryDetail ed)
        {
            switch (ed.Entry.SaveType)
            {
                case SaveType.Folder:
                    {
                        //文件夾地址轉爲相對地址
                        var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == ed.Entry.DbId).StoragePath;
                        ed.PathFolder = ed.PathFolder.Remove(s);
                        Core.Services.EntrySourceSerivce.AddEntrySource_PathFolder(ed.Entry.EntryId, ed.PathFolder, ed.Entry.DbId);
                        break;
                    }
                case SaveType.Files:
                    {
                        List<EntrySourceDb> entrySourceDbs = new List<EntrySourceDb>();
                        Core.Services.EntrySourceSerivce.AddEntrySource(entrySourceDbs, ed.Entry.DbId);
                        break;
                    }
                default:
                    break;
            }
        }
        /// <summary>
        /// 表Entry数据处理
        /// </summary>
        private static void TreatEntry(EntryDetail ed)
        {
            #region 词条表：  詞條路徑 and 封面路徑 取相對地址保存
            var coverType = Path.GetExtension(ed.FullCoverImgPath);//获取封面路径扩展名
            ed.Entry.CoverImg = Path.Combine(Services.ConfigService.InfoFolder, $"Cover{coverType}");//db封面路径 相对词条（存入数据库)
            ed.Entry.Path = PathService.EntryRelativePath(ed.Entry);//db词条路径 相对仓库（存入数据库)
            string newFullImgCoverPath = Path.Combine(ed.FullEntryPath, ed.Entry.CoverImg);//封面 全路径
            if (newFullImgCoverPath != ed.FullCoverImgPath)
                System.IO.File.Copy(ed.FullCoverImgPath, newFullImgCoverPath, true);//覆盖保存封面
            ed.FullCoverImgPath = newFullImgCoverPath;
            ed.Entry.MyRating = ed.Entry.MyRating <= 1 ? 1 : ed.Entry.MyRating;
            ed.Entry.CreateTime = ed.Entry.CreateTime <= new DateTime(1900, 1, 1) ? DateTime.Now : ed.Entry.CreateTime;
            ed.Entry.LastWatchTime= ed.Entry.LastWatchTime <= new DateTime(1900, 1, 1) ? DateTime.Now : ed.Entry.LastWatchTime;
            ed.Entry.LastUpdateTime = DateTime.Now;
            #endregion
        }
        private static void TreatEntryName(EntryDetail ed)
        {
            #region 词条表：  詞條路徑 and 封面路徑 取相對地址保存
            if (ed.Entry.Name == null)
                ed.Entry.Name = ed.Name;
            #endregion
        }
        
        /// <summary>
        /// 创建元数据(MataData)
        /// </summary>
        /// <param name="entryDetail"></param>
        private static void InitFile(Models.EntryDetail entryDetail)
        {
            string metaDateFile = Path.Combine(entryDetail.FullEntryPath, Services.ConfigService.MetadataFileNmae);
            Core.Models.EntryMetadata metadata;
            if (System.IO.File.Exists(metaDateFile))
            {
                metadata = Core.Models.EntryMetadata.Read(metaDateFile);
                metadata.Name = entryDetail.Entry.Name;
            }
            else
            {
                metadata = new Core.Models.EntryMetadata()
                {
                    Id = entryDetail.Entry.EntryId,
                    Name = entryDetail.Entry.Name,
                };
            }
            metadata.Save(metaDateFile);
        }
        /// <summary>
        /// 创建词条(Entry)路径 (仓库 -> .omdb -> Db源 -> 词条)
        /// </summary>
        /// <param name="entry"></param>
        private static void InitFolder(Models.EntryDetail entry)
        {
            Directory.CreateDirectory(entry.FullEntryPath);
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.AudioFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.ImgFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.VideoFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.ResourceFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.SubFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.InfoFolder));
            Directory.CreateDirectory(Path.Combine(entry.FullEntryPath, Services.ConfigService.MoreFolder));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeBySilderValue(DateTime startDate, DateTime endDate, int intValue)
        {
            // 将整数值映射到时间范围
            double totalDays = (endDate - startDate).TotalDays;
            double mappedDays = (double)intValue / 275 * totalDays;
            DateTimeOffset mappedDate = startDate.AddDays(mappedDays);
            return mappedDate.DateTime;
        }
        /// <summary>
        /// 
        /// </summary>
        public static MaxMinDateModel MaxMinDateModel { get; set; }
    }
}

