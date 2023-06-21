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

namespace OMDb.WinUI3.Services
{
    public static class EntryService
    {
        /// <summary>
        /// 新建词条
        /// </summary>
        /// <returns>词条唯一id</returns>
        public static async Task<string> AddEntryAsync()
        {
            var entryDetail = await Dialogs.EditEntryDialog.ShowDialog();
            if (entryDetail != null)
            {
                if (Directory.Exists(entryDetail.FullEntryPath))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPath = $"{entryDetail.FullEntryPath}({i++})";
                        if (!Directory.Exists(newPath))
                        {
                            entryDetail.FullEntryPath = newPath;
                            entryDetail.Entry.Path = newPath;
                            break;
                        }
                    }
                }

                //创建词条文件夹
                InitFolder(entryDetail);





                //创建元数据(MataData)
                InitFile(entryDetail);

                //保存至数据库
                await SaveToDbAsync(entryDetail);

                //这时已经是相对路径
                Helpers.InfoHelper.ShowSuccess("创建成功");
                GlobalEvent.NotifyAddEntry(null, new EntryEventArgs(entryDetail.Entry));
                return entryDetail.Entry.EntryId;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 创建元数据(MataData)
        /// </summary>
        /// <param name="entry"></param>
        private static void InitFile(Models.EntryDetail entry)
        {
            string metaDateFile = Path.Combine(entry.FullEntryPath, Services.ConfigService.MetadataFileNmae);
            Core.Models.EntryMetadata metadata;
            if (System.IO.File.Exists(metaDateFile))
            {
                metadata = Core.Models.EntryMetadata.Read(metaDateFile);
                metadata.Name = entry.Entry.Name;
            }
            else
            {
                metadata = new Core.Models.EntryMetadata()
                {
                    Id = entry.Entry.EntryId,
                    Name = entry.Entry.Name,
                };
            }
            metadata.Save(metaDateFile);
        }


        //创建词条(Entry)路径 (仓库 -> .omdb -> Db源 -> 词条)
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
        /// 保存至数据库
        /// </summary>
        /// <param name="ed"></param>
        /// <param name="entryNames"></param>
        /// <returns></returns>
        private static async Task SaveToDbAsync(Models.EntryDetail ed)
        {
            await Task.Run(() =>
            {
                Core.Services.EntryService.AddEntry(ed.Entry);//词条
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(ed.Entry.EntryId, ed.Entry.DbId, ed.Entry.Name);//更新或插入词条默认名称



                TreatEntry(ed);//处理词条主表Entry
                TreatEntrySource(ed);//资源路径表EntrySource更新
                TreatLabelLink(ed);//词条标签关联表

            });
        }


        /// <summary>
        /// 保存至数据库
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="entryNames"></param>
        /// <returns></returns>
        private static async Task UpdateDbAsync(Models.EntryDetail ed)
        {
            await Task.Run(() =>
            {
                //数据库主表and多语言表 更新
                Core.Services.EntryService.UpdateEntry(ed.Entry);//词条
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(ed.Entry.EntryId, ed.Entry.DbId, ed.Entry.Name);//更新或插入词条默认名称

                TreatEntry(ed);//处理词条主表Entry
                TreatEntrySource(ed);//资源路径表EntrySource更新
                TreatLabelLink(ed);//词条标签关联表

            });
        }

        /// <summary>
        /// 编辑词条
        /// </summary>
        /// <returns>词条唯一id</returns>
        public static async Task EditEntryAsync(Core.Models.Entry entry)
        {
            var entryDetail = await Dialogs.EditEntryDialog.ShowDialog(entry);
            if (entryDetail != null)
            {
                InitFile(entryDetail);
                await UpdateDbAsync(entryDetail);
                Helpers.InfoHelper.ShowSuccess("已保存");
                GlobalEvent.NotifyUpdateEntry(null, new EntryEventArgs(entryDetail.Entry));
            }
        }

        public static async Task RemoveEntryAsync(Core.Models.Entry entry)
        {
            if (await Dialogs.QueryDialog.ShowDialog("删除", $"是否删除 {entry.Name} 词条?不会删除本地文件"))
            {
                Core.Services.EntryService.RemoveEntry(entry);
                Helpers.InfoHelper.ShowSuccess("已删除");
                GlobalEvent.NotifyRemoveEntry(null, new EntryEventArgs(entry));
            }
        }

        private static void TreatLabelLink(EntryDetail ed)
        {
            if (ed.Labels?.Count != 0)
            {
                List<Core.DbModels.EntryLabelClassLinkDb> entryLabelDbs = new List<Core.DbModels.EntryLabelClassLinkDb>(ed.Labels.Count);
                ed.Labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelClassLinkDb() { EntryId = ed.Entry.EntryId, LCId = p.LCId, DbId = ed.Entry.DbId }));
                Core.Services.LabelService.ClearEntryLabel(ed.Entry.EntryId);//清空词条分类标签
                Core.Services.LabelService.AddEntryLabel(entryLabelDbs);//添加词条分类标签
            }
            if (ed.Lpdbs?.Count != 0)
            {
                List<Core.DbModels.EntryLabelPropertyLinkDb> entryLabelPropertyDbs = new List<Core.DbModels.EntryLabelPropertyLinkDb>(ed.Lpdbs.Count);
                ed.Lpdbs.ForEach(p => entryLabelPropertyDbs.Add(new Core.DbModels.EntryLabelPropertyLinkDb() { EntryId = ed.Entry.EntryId, LPId = p.LPId, DbId = ed.Entry.DbId }));
                Core.Services.LabelPropertyService.ClearEntryLabel(ed.Entry.EntryId);//清空词条属性标签
                Core.Services.LabelPropertyService.AddEntryLabel(entryLabelPropertyDbs);//添加词条属性标签
            }
        }
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
            #endregion
        }



        /// <summary>
        /// 批量新增
        /// </summary>
        /// <returns></returns>
        public static async Task<string> AddEntryBatchAsync()
        {
            var entryDetail = await Dialogs.AddEntryBatchDialog.ShowDialog();
            return null;
        }
    }
}
