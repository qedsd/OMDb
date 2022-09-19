using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class EditEntryViewModel: ObservableObject
    {
        public Models.EntryDetail EntryDetail { get; set; }
        public Core.Models.Entry Entry { get; set; }
        //private List<Models.EntryName> entryNames;
        //public List<Models.EntryName> EntryNames
        //{
        //    get => entryNames;
        //    set
        //    {
        //        SetProperty(ref entryNames, value);
        //    }
        //}
        //private Models.EntryName entryName;
        //public Models.EntryName EntryName
        //{
        //    get => entryName;
        //    set => SetProperty(ref entryName, value);
        //}
        private string entryName;
        public string EntryName
        {
            get => entryName;
            set => SetProperty(ref entryName, value);
        }
        private string entryPath;
        public string EntryPath
        {
            get => entryPath;
            set
            {
                SetProperty(ref entryPath, value);
                Entry.Path = value;
            }
        }
        private DateTimeOffset releaseDate = DateTimeOffset.Now;
        public DateTimeOffset ReleaseDate
        {
            get => releaseDate;
            set
            {
                SetProperty(ref releaseDate, value);
                Entry.ReleaseDate = value;
            }
        }
        private double myRating = -1;
        public double MyRating
        {
            get => myRating;
            set
            {
                SetProperty(ref myRating, value);
                Entry.MyRating = value;
            }
        }
        public List<Models.EnrtyStorage> EnrtyStorages { get; set; }
        private Models.EnrtyStorage selectedEnrtyStorage;
        public Models.EnrtyStorage SelectedEnrtyStorage
        {
            get => selectedEnrtyStorage;
            set
            {
                SetProperty(ref selectedEnrtyStorage, value);
                SelectedEntryDicPath = Path.Combine(Path.GetDirectoryName(selectedEnrtyStorage.StoragePath), Services.ConfigService.DefaultEntryFolder);//重置为默认路径
                //if (EntryName != null&&!string.IsNullOrEmpty(EntryName.Name))
                //{
                //    SetEntryPath(EntryName.Name);
                //}
                SetEntryPath(EntryName);
            }
        }
        private string selectedEntryDicPath = string.Empty;
        /// <summary>
        /// 文件夹选取的词条存储路径
        /// 完整路径，必须在选中的仓库路径下
        /// </summary>
        public string SelectedEntryDicPath
        {
            get => selectedEntryDicPath;
            set
            {
                selectedEntryDicPath = value;
                //SetEntryPath(EntryNames.FirstOrDefault(p=>p.IsDefault)?.Name);
                SetEntryPath(EntryName);
            }
        }
        private List<Models.Label> labels;
        /// <summary>
        /// 绑定的标签
        /// </summary>
        public List<Models.Label> Labels
        {
            get => labels;
            set=>SetProperty(ref labels, value);
        }
        public void SetEntryPath(string name)
        {
            if (SelectedEntryDicPath != null && !string.IsNullOrEmpty(name))
            {
                EntryPath = System.IO.Path.Combine(SelectedEntryDicPath, name);
            }
        }

        public EditEntryViewModel(Core.Models.Entry entry)
        {
            //EntryNames = new List<Models.EntryName>();
            if (entry == null)
            {
                Entry = new Core.Models.Entry();
                Entry.Id = Guid.NewGuid().ToString();
                Entry.CreateTime = DateTime.Now;
                Entry.LastUpdateTime = DateTime.Now;
                Entry.ReleaseDate = DateTimeOffset.Now;
                //foreach (Core.Enums.LangEnum p in Enum.GetValues(typeof(Core.Enums.LangEnum)))
                //{
                //    EntryNames.Add(new Models.EntryName()
                //    {
                //        Lang = p,
                //        IsDefault = p == Core.Enums.LangEnum.zh_CN,
                //    });
                //}
                //EntryName = EntryNames.FirstOrDefault();
                EnrtyStorages = Services.ConfigService.EnrtyStorages.Where(p => p.StoragePath != null).ToList();
            }
            else
            {
                Entry = entry.DepthClone<Core.Models.Entry>();
                //拼接全封面路径、存储路径
                Entry.Path = Helpers.PathHelper.EntryFullPath(Entry);
                Entry.CoverImg = Helpers.PathHelper.EntryCoverImgFullPath(Entry);
                //现有词条暂不允许修改仓库
                var onlyStorage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p=>p.StorageName == Entry.DbId);
                if(onlyStorage != null)
                {
                    EnrtyStorages = new List<Models.EnrtyStorage>() { onlyStorage };
                }
                MyRating = Entry.MyRating == null ? -1 : (double)Entry.MyRating;
            }
            
            SelectedEnrtyStorage = EnrtyStorages?.FirstOrDefault();
            Init(entry);
        }
        private async void Init(Core.Models.Entry entry)
        {
            if(entry == null)
            {
                Labels = new List<Models.Label>();
            }
            else
            {
                var labels = await Core.Services.LabelService.GetLabelOfEntryAsync(entry.DbId,entry.Id);
                if (labels != null)
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        Labels = new List<Models.Label>(labels.Select(p=>new Models.Label(p)));
                    });
                }
                else
                {
                    Labels = new List<Models.Label>();
                }
                var names = await Core.Services.EntryNameSerivce.QueryNamesAsync(entry.Id, entry.DbId);
                if (names != null)
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        //foreach (Core.Enums.LangEnum p in Enum.GetValues(typeof(Core.Enums.LangEnum)))
                        //{
                        //    var targetName = names.FirstOrDefault(p2 => p2.Lang == p.ToString());
                        //    EntryNames.Add(new Models.EntryName()
                        //    {
                        //        Lang = p,
                        //        IsDefault = p == Core.Enums.LangEnum.zh_CN,
                        //        Name = targetName?.Name
                        //    });
                        //}
                        //EntryName = EntryNames.FirstOrDefault(p=>p.IsDefault);
                        EntryName = names.FirstOrDefault(p => p.IsDefault)?.Name;
                    });
                }
            }
        }

        //public ICommand CheckDefaultCommand => new RelayCommand(() =>
        //{
        //    if (EntryName != null)
        //    {
        //        EntryNames.ForEach(p =>
        //        {
        //            if (p.IsDefault && p != EntryName)
        //            {
        //                p.IsDefault = false;
        //            }
        //        });
        //        SetEntryPath(EntryName.Name);
        //    }
        //});
    }
}
