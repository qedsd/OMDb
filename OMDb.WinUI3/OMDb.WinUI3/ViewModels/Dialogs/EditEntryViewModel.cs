using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.Services.Settings;
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
    public class EditEntryViewModel : ObservableObject
    {
        public Models.EntryDetail EntryDetail { get; set; }
        public Core.Models.Entry Entry { get; set; }

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
                SelectedEntryDicPath = selectedEnrtyStorage.StoragePath;//重置为默认路径
                SetFullEntryPathByName(EntryName);
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
                SetFullEntryPathByName(EntryName);
            }
        }
        private List<Models.Label> labels;
        /// <summary>
        /// 绑定的标签
        /// </summary>
        public List<Models.Label> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value);
        }

        private ObservableCollection<Models.LabelProperty> _label_Property=new ObservableCollection<Models.LabelProperty>();
        /// <summary>
        /// 绑定的标签
        /// </summary>
        public ObservableCollection<Models.LabelProperty> Label_Property
        {
            get => _label_Property;
            set => SetProperty(ref _label_Property, value);
        }

        public void SetFullEntryPathByName(string name)
        {
            if (SelectedEntryDicPath != null && !string.IsNullOrEmpty(name))
            {
                EntryPath = PathService.GetFullEntryPathByEntryName( name, selectedEnrtyStorage.StoragePath);
            }
        }
        public void SetFullEntryPathByRelativePath(string relativePath)
        {
            if (SelectedEntryDicPath != null && !string.IsNullOrEmpty(relativePath))
            {
                EntryPath =  selectedEnrtyStorage.StoragePath+ relativePath;
            }
        }
        public EditEntryViewModel(Core.Models.Entry entry)
        {
            //EntryNames = new List<Models.EntryName>();
            if (entry == null)
            {
                Entry = new Core.Models.Entry();
                Entry.EntryId = Guid.NewGuid().ToString();
                Entry.CreateTime = DateTime.Now;
                Entry.LastUpdateTime = DateTime.Now;
                Entry.ReleaseDate = DateTimeOffset.Now;
                EnrtyStorages = Services.ConfigService.EnrtyStorages.Where(p => p.StoragePath != null).ToList();
            }
            else
            {
                Entry = entry.DepthClone<Core.Models.Entry>();
                //拼接全封面路径、存储路径
                Entry.Path = PathService.EntryFullPath(Entry);
                Entry.CoverImg = PathService.EntryCoverImgFullPath(Entry);
                //现有词条暂不允许修改仓库
                var onlyStorage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == Entry.DbId);
                if (onlyStorage != null)
                {
                    EnrtyStorages = new List<Models.EnrtyStorage>() { onlyStorage };
                }
                MyRating = Entry.MyRating == null ? -1 : (double)Entry.MyRating;
            }

            SelectedEnrtyStorage = EnrtyStorages?.FirstOrDefault();
            Init(entry);

            Events.GlobalEvent.UpdateLPSEvent += GlobalEvent_UpdateLPSEvent;
        }
        private void GlobalEvent_UpdateLPSEvent(object sender, Events.LPSEventArgs lpe)
        {
            var lst_label_lp = Core.Services.LabelPropertyService.GetAllLabel(DbSelectorService.dbCurrentId);
            List<string> lpBabaIds= new List<string>();
            foreach (var item in lpe.LPS)
            {
                var lst_LK = Core.Services.LabelPropertyService.GetLKId(DbSelectorService.dbCurrentId, item.LPDb.LPId);
                foreach (var lkid in lst_LK)
                {
                    lpBabaIds.Add(lst_label_lp.Where(a => a.LPId.Equals(lkid)).FirstOrDefault().ParentId);                
                }
                lpBabaIds = lpBabaIds.Distinct().ToList();
                foreach (var lpbabaid in lpBabaIds)
                {
                    lst_label_lp.Where(a => a.ParentId.Equals(lpbabaid)).Where(a => lst_LK.Contains(a.LPId));
                }
                foreach (var lpdb in lst_label_lp)
                {
                    if (lpBabaIds.Contains(lpdb.ParentId))
                    {
                        if (!lst_LK.Contains(lpdb.LPId))
                        {
                            Label_Property.Where(a=>a.LPDb.LPId== lpdb.LPId).FirstOrDefault().IsHiden = true;
                        }
                    }
                }
            }
        }

        private async void Init(Core.Models.Entry entry)
        {
            if (entry == null)
            {
                Labels = new List<Models.Label>();
            }
            else
            {
                var labels = await Core.Services.LabelService.GetLabelOfEntryAsync(entry.EntryId);
                if (labels != null)
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        Labels = new List<Models.Label>(labels.Select(p => new Models.Label(p)));
                    });
                }
                else
                {
                    Labels = new List<Models.Label>();
                }
                var names = await Core.Services.EntryNameSerivce.QueryNamesAsync(entry.EntryId, entry.DbId);
                if (names != null)
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        EntryName = names.FirstOrDefault(p => p.IsDefault)?.Name;
                    });
                }
            }
        }
    }
}
