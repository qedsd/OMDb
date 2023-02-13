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
                            break;
                        }
                    }
                }

                //创建词条文件夹
                InitFolder(entryDetail);

                //复制封面图(Cover)、并同步修改封面路径
                var coverType = Path.GetFileName(entryDetail.FullCoverImgPath).SubString_A21(".", 1, false);
                string newImgCoverPath = Path.Combine(entryDetail.FullEntryPath, Services.ConfigService.InfoFolder, "Cover" + coverType);
                if (newImgCoverPath != entryDetail.FullCoverImgPath) { File.Copy(entryDetail.FullCoverImgPath, newImgCoverPath, true); }                
                entryDetail.FullCoverImgPath = newImgCoverPath;

                //创建元数据(MataData)
                InitFile(entryDetail);

                //保存至数据库
                await SaveToDbAsync(entryDetail);

                //这时已经是相对路径
                Helpers.InfoHelper.ShowSuccess("创建成功");
                GlobalEvent.NotifyAddEntry(null,new EntryEventArgs(entryDetail.Entry));
                return entryDetail.Entry.Id;
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
                    Id = entry.Entry.Id,
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
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(entry.Entry.Id, entry.Entry.DbId, entry.Entry.Name);//更新或插入词条默认名称

                //添加标签
                if (entry.Labels?.Count != 0)
                {
                    List<Core.DbModels.EntryLabelDb> entryLabelDbs = new List<Core.DbModels.EntryLabelDb>(entry.Labels.Count);
                    entry.Labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelDb() { EntryId = entry.Entry.Id, LabelId = p.Id,DbId = entry.Entry.DbId }));
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
        private static async Task UpdateDbAsync(Models.EntryDetail entry)
        {
            await Task.Run(() =>
            {
                Core.Services.EntryService.UpdateEntry(entry.Entry);//词条
                Core.Services.EntryNameSerivce.UpdateOrAddDefaultNames(entry.Entry.Id, entry.Entry.DbId, entry.Entry.Name);//更新或插入词条默认名称
                if (entry.Labels?.Count != 0)
                {
                    List<Core.DbModels.EntryLabelDb> entryLabelDbs = new List<Core.DbModels.EntryLabelDb>(entry.Labels.Count);
                    entry.Labels.ForEach(p => entryLabelDbs.Add(new Core.DbModels.EntryLabelDb() { EntryId = entry.Entry.Id, LabelId = p.Id,DbId = entry.Entry.DbId }));
                    Core.Services.LabelService.ClearEntryLabel(entry.Entry.Id);//清空词条标签
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
            if(await Dialogs.QueryDialog.ShowDialog("删除",$"是否删除 {entry.Name} 词条?不会删除本地文件"))
            {
                Core.Services.EntryService.RemoveEntry(entry);
                Helpers.InfoHelper.ShowSuccess("已删除");
                GlobalEvent.NotifyRemoveEntry(null, new EntryEventArgs(entry));
            }
        }
    }
}
