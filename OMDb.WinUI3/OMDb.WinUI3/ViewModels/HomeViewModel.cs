using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class HomeViewModel
    {
        public ICommand AddEntryCommand => new RelayCommand(async() =>
        {
            if(Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                var result = await Dialogs.EditEntryDialog.ShowDialog();
                if (result != null && result.Item1 != null && result.Item2 != null)
                {
                    var entry = result.Item1;
                    if (Directory.Exists(entry.Path))
                    {
                        int i = 1;
                        while (true)
                        {
                            entry.Path = $"{entry.Path}({i})";
                            if (!Directory.Exists(entry.Path))
                            {
                                break;
                            }
                        }
                    }
                    //此时的entry封面图、存储路径都是完整路径
                    //创建文件夹、元文件、复制封面、保存数据库
                    InitFolder(entry);
                    InitFile(entry);
                    await SaveToDbAsync(entry, result.Item2);
                    //这时已经是相对路径
                }
            }
        });
        /// <summary>
        /// 复制封面图、元数据
        /// 会将entry.CoverImg修改为最终路径
        /// </summary>
        /// <param name="entry"></param>
        private static void InitFile(Core.Models.Entry entry)
        {
            string newImgCoverPath = Path.Combine(entry.Path, Services.ConfigService.ImgFolder, Path.GetFileName(entry.CoverImg));
            File.Copy(entry.CoverImg, newImgCoverPath, true);
            entry.CoverImg = newImgCoverPath;
            Core.Models.EntryMetadata metadata = new Core.Models.EntryMetadata()
            {
                Id = entry.Id,
                Name = entry.Name,
            };
            metadata.Save(Path.Combine(entry.Path, Services.ConfigService.MetadataFileNmae));
        }
        private static void InitFolder(Core.Models.Entry entry)
        {
            Directory.CreateDirectory(entry.Path);
            Directory.CreateDirectory(Path.Combine(entry.Path, Services.ConfigService.SubFolder));
            Directory.CreateDirectory(Path.Combine(entry.Path, Services.ConfigService.ImgFolder));
            Directory.CreateDirectory(Path.Combine(entry.Path, Services.ConfigService.VideoFolder));
            Directory.CreateDirectory(Path.Combine(entry.Path, Services.ConfigService.ResourceFolder));
        }
        private static async Task SaveToDbAsync(Core.Models.Entry entry,List<Models.EntryName> entryNames)
        {
            //恢复相对路径
            entry.CoverImg = Helpers.PathHelper.EntryCoverImgRelativePath(entry);
            entry.Path = Helpers.PathHelper.EntryRelativePath(entry);
            //保存至数据库
            Core.Services.EntryService.AddEntry(entry);
            List<Core.DbModels.EntryNameDb> entryNameDbs = new List<Core.DbModels.EntryNameDb>();
            foreach (var p in entryNames.Where(p=>p.Name != null))
            {
                entryNameDbs.Add(new Core.DbModels.EntryNameDb()
                {
                    Id = entry.Id,
                    Name = p.Name,
                    Lang = p.Lang.ToString(),
                    IsDefault = p.IsDefault,
                });
            }
            await Core.Services.EntryNameSerivce.AddNamesAsync(entryNameDbs, entry.DbId);
        }
    }
}
