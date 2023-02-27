using CommunityToolkit.WinUI.UI.Controls;
using NPOI.POIFS.FileSystem;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.WinUI3.Events;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                //數據庫 詞條路徑&圖片路徑 取相對地址
                var coverType = Path.GetExtension(entryDetail.FullCoverImgPath);
                entryDetail.Entry.CoverImg = Path.Combine(Services.ConfigService.InfoFolder, $@"Cover{coverType}");
                entryDetail.Entry.Path = PathService.EntryRelativePath(entryDetail.Entry);
                //复制封面图(Cover)、并同步修改封面路径
                string newFullImgCoverPath = Path.Combine(entryDetail.FullEntryPath, entryDetail.Entry.CoverImg);
                if (newFullImgCoverPath != entryDetail.FullCoverImgPath) { File.Copy(entryDetail.FullCoverImgPath, newFullImgCoverPath, true); }
                entryDetail.FullCoverImgPath = newFullImgCoverPath;


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
        /// <param name="entry"></param>
        /// <param name="entryNames"></param>
        /// <returns></returns>
        private static async Task SaveToDbAsync(Models.EntryDetail entry)
        {
            await Task.Run(() =>
            {
                Core.Services.EntryService.AddEntry(entry.Entry);//词条
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(entry.Entry.EntryId, entry.Entry.DbId, entry.Entry.Name);//更新或插入词条默认名称
                switch (entry.Entry.SaveType)
                {
                    case '1':
                        {
                            //文件夾地址轉爲相對地址
                            var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.Entry.DbId).StoragePath;
                            entry.PathFolder = entry.PathFolder.Replace(s, null);
                            Core.Services.EntrySourceSerivce.AddEntrySource_PathFolder(entry.Entry.EntryId, entry.PathFolder, entry.Entry.DbId);
                            break;
                        }
                    case '2':
                        {
                            break;
                        }
                    default:
                        break;
                }
                //
                //添加标签
                if (entry.Labels?.Count != 0)
                {
                    List<Core.DbModels.EntryLabelDb> entryLabelDbs = new List<Core.DbModels.EntryLabelDb>(entry.Labels.Count);
                    entry.Labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelDb() { EntryId = entry.Entry.EntryId, LabelId = p.Id, DbId = entry.Entry.DbId }));
                    Core.Services.LabelService.AddEntryLabel(entryLabelDbs);//添加词条标签
                }
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

                //封面变更
                var coverType = Path.GetExtension(ed.Entry.CoverImg);
                string newImgCoverPath = Path.Combine(ed.FullEntryPath, Services.ConfigService.InfoFolder, "Cover" + coverType);
                File.Copy(ed.Entry.CoverImg, newImgCoverPath, true);
                ed.Entry.CoverImg = ed.Entry.CoverImg.Replace(ed.Entry.CoverImg.SubString_A2B(@"\", ".", 1, 1, true, false), @"\Cover.");

                //數據庫 詞條路徑&圖片路徑 取相對地址
                ed.Entry.CoverImg = Path.Combine(Services.ConfigService.InfoFolder, Path.GetFileName(ed.Entry.CoverImg));
                ed.Entry.Path = PathService.EntryRelativePath(ed.Entry);

                //数据库表Entry3更新
                if (ed.Entry.SaveType.Equals('1'))
                {
                    var s = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == ed.Entry.DbId).StoragePath;
                    ed.PathFolder = ed.PathFolder.Replace(s, null);
                    Core.Services.EntrySourceSerivce.UpdateEntrySource_PathFolder(ed.Entry.EntryId, ed.PathFolder, ed.Entry.DbId);
                }
                else if (ed.Entry.SaveType.Equals('2'))
                {
                    List<EntrySourceDb> entrySourceDbs = new List<EntrySourceDb>();
                    Core.Services.EntrySourceSerivce.AddEntrySource(entrySourceDbs, ed.Entry.DbId);
                }

                //数据库Entry1、Entry2更新
                Core.Services.EntryService.UpdateEntry(ed.Entry);//词条
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(ed.Entry.EntryId, ed.Entry.DbId, ed.Entry.Name);//更新或插入词条默认名称
                if (ed.Labels?.Count != 0)
                {
                    List<Core.DbModels.EntryLabelDb> entryLabelDbs = new List<Core.DbModels.EntryLabelDb>(ed.Labels.Count);
                    ed.Labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelDb() { EntryId = ed.Entry.EntryId, LabelId = p.Id, DbId = ed.Entry.DbId }));
                    Core.Services.LabelService.ClearEntryLabel(ed.Entry.EntryId);//清空词条标签
                    Core.Services.LabelService.AddEntryLabel(entryLabelDbs);//添加词条标签
                }
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
    }
}
