using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Utils.Extensions;
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
    public class EditEntryHomeViewModel : ObservableObject
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
        private List<Models.LabelClass> labels;
        /// <summary>
        /// 绑定的标签
        /// </summary>
        public List<Models.LabelClass> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value);
        }

        private ObservableCollection<Models.LabelProperty> _label_Property = new ObservableCollection<Models.LabelProperty>();
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
                EntryPath = PathService.GetFullEntryPathByEntryName(name, selectedEnrtyStorage.StoragePath);
            }
        }
        public void SetFullEntryPathByRelativePath(string relativePath)
        {
            if (SelectedEntryDicPath != null && !string.IsNullOrEmpty(relativePath))
            {
                EntryPath = System.IO.Path.Combine(selectedEnrtyStorage.StoragePath, relativePath);
            }
        }
        public EditEntryHomeViewModel(Core.Models.Entry entry)
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
            var lpsChecked = lpe.LPS.Where(a => a.IsChecked);
            var lpAll = Core.Services.LabelPropertyService.GetAllLabel(DbSelectorService.dbCurrentId);
            var lpAllParents = lpAll.Where(a => a.Level == 1);
            if (lpsChecked.Count() <= 0)
            {
                foreach (var lp in Label_Property)
                {
                    if (!lpe.LPS.Select(a => a.LPDb.LPID).Contains(lp.LPDb.LPID))
                    {
                        lp.IsHiden = false;
                    }
                }
            }
            else
            {
                #region 先全部隐藏
                var lpAllParentLinks = new List<string>();//所有需要隐藏的标签 属性标题 初始化
                foreach (var lpChecked in lpsChecked)
                {
                    var lpParentLinks = Core.Services.LabelPropertyService.GetLinkId(DbSelectorService.dbCurrentId, lpChecked.LPDb.ParentId);//获取该标签属性数据（父）的关联信息
                    lpAllParentLinks = lpAllParentLinks.Union(lpParentLinks).Distinct().ToList();
                }
                foreach (var lp in lpAll)
                {
                    if (lpAllParentLinks.Contains(lp.ParentId))
                    {
                        Label_Property.Where(a => a.LPDb.LPID == lp.LPID).FirstOrDefault().IsHiden = true;
                    }
                }
                #endregion

                #region 显示关联数据
                foreach (var lpChecked in lpsChecked)
                {
                    var lpDataLinks = Core.Services.LabelPropertyService.GetLinkId(DbSelectorService.dbCurrentId, lpChecked.LPDb.LPID);//获取该标签属性数据（子）的关联信息
                    var lpParentLinks = Core.Services.LabelPropertyService.GetLinkId(DbSelectorService.dbCurrentId, lpChecked.LPDb.ParentId);//获取该标签属性标题（父）的关联信息

                    //该数据没有数据关联，显示所有父级关联的数据
                    if (lpDataLinks.Count == 0)
                    {
                        foreach (var lpParentLink in lpParentLinks)
                        {
                            foreach (var item in Label_Property.Where(a => lpParentLinks.Contains(a.LPDb.ParentId)))
                            {
                                item.IsHiden = false;
                            }
                        }
                    }


                    var lpChildren = lpAll.Where(a => lpParentLinks.Contains(a.ParentId)).Where(a => lpDataLinks.Contains(a.LPID));

                    foreach (var lpdb in lpChildren)
                    {
                        Label_Property.Where(a => a.LPDb.LPID == lpdb.LPID).FirstOrDefault().IsHiden = false;
                    }
                }
                #endregion
            }
        }

        private async void Init(Core.Models.Entry entry)
        {
            if (entry == null)
            {
                Labels = new List<Models.LabelClass>();
            }
            else
            {
                var labels = await Core.Services.LabelClassService.GetLabelOfEntryAsync(entry.EntryId);
                if (labels != null)
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        Labels = new List<Models.LabelClass>(labels.Select(p => new Models.LabelClass(p)));
                    });
                }
                else
                {
                    Labels = new List<Models.LabelClass>();
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
