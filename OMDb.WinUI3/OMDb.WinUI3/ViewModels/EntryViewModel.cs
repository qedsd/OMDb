using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class EntryViewModel : ObservableObject
    {
        public EntryViewModel()
        {
            InitEnumItemsource();
            sortType = Core.Enums.SortType.LastUpdateTime;
            sortWay = Core.Enums.SortWay.Positive;
            Init();
            Current = this;
            Events.GlobalEvent.UpdateEntryEvent += GlobalEvent_UpdateEntryEvent;
            Events.GlobalEvent.AddEntryEvent += GlobalEvent_AddEntryEvent;
            Events.GlobalEvent.RemoveEntryEvent += GlobalEvent_RemoveEntryEvent;
        }
        public static EntryViewModel Current { get; private set; }

        #region 字段
        public ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages;
        private ObservableCollection<Core.Models.Entry> _entries;
        public ObservableCollection<Core.Models.Entry> Entries
        {
            get => _entries;
            set
            {
                SetProperty(ref _entries, value);
            }
        }
        private ObservableCollection<Label> _labels;
        public ObservableCollection<Label> Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }

        private Core.Enums.SortType sortType = Core.Enums.SortType.LastUpdateTime;
        public Core.Enums.SortType SortType
        {
            get => sortType;
            set
            {
                SetProperty(ref sortType, value);
                _ = UpdateEntryListAsync();
            }
        }
        private Core.Enums.SortWay sortWay = Core.Enums.SortWay.Positive;
        public Core.Enums.SortWay SortWay
        {
            get => sortWay;
            set
            {
                SetProperty(ref sortWay, value);
                _ = UpdateEntryListAsync();
            }
        }
        public List<string> SortTypeStrs { get; set; }
        private int sortTypeIndex = 0;
        public int SortTypeIndex
        {
            get => sortTypeIndex;
            set
            {
                SetProperty(ref sortTypeIndex, value);
                SortType = (Core.Enums.SortType)value;
            }
        }
        private int sortWayIndex = 0;
        public int SortWayIndex
        {
            get => sortWayIndex;
            set
            {
                SetProperty(ref sortWayIndex, value);
                SortWay = (Core.Enums.SortWay)value;
            }
        }
        public List<string> SortWayStrs { get; set; }

        private ObservableCollection<EnrtyStorage> entryStorages;
        public ObservableCollection<EnrtyStorage> EntryStorages
        {
            get => entryStorages;
            set => SetProperty(ref entryStorages, value);
        }

        private string autoSuggestText;
        public string AutoSuggestText
        {
            get => autoSuggestText;
            set
            {
                SetProperty(ref autoSuggestText, value);
                UpdateSuggest(value);
            }
        }
        private List<Core.Models.QueryResult> autoSuggestItems;
        public List<Core.Models.QueryResult> AutoSuggestItems
        {
            get => autoSuggestItems;
            set
            {
                SetProperty(ref autoSuggestItems, value);
            }
        }
        private Core.Models.QueryResult autoSuggestItem;
        /// <summary>
        /// 搜索框选中项
        /// </summary>
        public Core.Models.QueryResult AutoSuggestItem
        {
            get => autoSuggestItem;
            set
            {
                SetProperty(ref autoSuggestItem, value);
                ConfirmSuggest(value.Value as string);
            }
        }
        private bool isFilterLabel = false;
        /// <summary>
        /// 是否按选择的标签进行筛选
        /// </summary>
        public bool IsFilterLabel
        {
            get => isFilterLabel;
            set
            {
                SetProperty(ref isFilterLabel, value);
                _ = UpdateEntryListAsync();
            }
        }

        private ObservableCollection<EntrySortInfoTree> _entrySortInfoTrees = new ObservableCollection<EntrySortInfoTree>();
        public ObservableCollection<EntrySortInfoTree> EntrySortInfoTrees
        {
            get => _entrySortInfoTrees;
            set => SetProperty(ref _entrySortInfoTrees, value);
        }

        private EntrySortInfoTree _entrySortInfoCurrents;
        public EntrySortInfoTree EntrySortInfoCurrents
        {
            get => _entrySortInfoCurrents;
            set => SetProperty(ref _entrySortInfoCurrents, value);
        }


        private ObservableCollection<EntrySortInfoResult> _entrySortInfoResults = new ObservableCollection<EntrySortInfoResult>();
        public ObservableCollection<EntrySortInfoResult> EntrySortInfoResults
        {
            get => _entrySortInfoResults;
            set => SetProperty(ref _entrySortInfoResults, value);
        }
        #endregion

        #region ICommand
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
            Helpers.InfoHelper.ShowMsg("刷新完成");
        });
        public ICommand ItemClickCommand => new RelayCommand<Core.Models.Entry>((entry) =>
        {
            TabViewService.AddItem(new Views.EntryDetailPage(entry));
        });
        public ICommand LabelChangedCommand => new RelayCommand<IEnumerable<Models.Label>>((items) =>
        {
            _ = UpdateEntryListAsync();
        });
        public ICommand EntryStorageChangedCommand => new RelayCommand<IEnumerable<Models.EnrtyStorage>>((items) =>
        {
            _ = UpdateEntryListAsync();
        });
        public ICommand QuerySubmittedCommand => new RelayCommand<Core.Models.QueryResult>(async (item) =>
        {
            if (item == null)
            {
                if (string.IsNullOrEmpty(AutoSuggestText))
                {
                    _ = UpdateEntryListAsync();
                }
                else
                {
                    List<Core.Models.Entry> items = new List<Core.Models.Entry>();
                    foreach (var p in AutoSuggestItems.GroupBy(p => p.DbId))
                    {
                        var entryIds = p.Select(p => p.Id).ToList().Distinct();
                        items.AddRange(await Core.Services.EntryService.GetEntryByIdsAsync(entryIds, p.Key));
                    }
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        Entries = items.ToObservableCollection();
                    });
                }
            }
            else
            {
                AutoSuggestItem = item;
            }
        });
        public ICommand AddEntryCommand => new RelayCommand(async () =>
        {
            Services.ConfigService.LoadStorages();
            if (Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                await Services.EntryService.AddEntryAsync();
            }
        });
        public ICommand AddEntryBatchCommand => new RelayCommand(async () =>
        {
            Services.ConfigService.LoadStorages();
            if (Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                await Services.EntryService.AddEntryBatchAsync();
            }
        });

        public ICommand AddEntrySortInfoCommand => new RelayCommand(() =>
        {
            EntrySortInfoResult ESIR = new EntrySortInfoResult(EntrySortInfoCurrents.Title);
            EntrySortInfoResults.Add(ESIR);
        });
        public ICommand RemoveEntrySortInfoCommand => new RelayCommand(() =>
        {

        });
        public ICommand ClearEntrySortInfoCommand => new RelayCommand(() =>
        {

        });
        #endregion

        #region Methon
        private async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();
            var labelDbs = await Core.Services.LabelClassService.GetAllLabelAsync(Services.Settings.DbSelectorService.dbCurrentId);
            List<Label> labels = null;
            if (labelDbs != null)
            {
                labels = labelDbs.Select(p => new Label(p)).ToList();
                Labels = new ObservableCollection<Label>(labels);
            }
            EntryStorages = ConfigService.EnrtyStorages;
            foreach (var item in EntryStorages)
            {
                item.IsChecked = true;
            }
            await UpdateEntryListAsync();
            Helpers.InfoHelper.HideWaiting();

            #region 排序模式加载

            EntrySortInfoTree eit_base = new EntrySortInfoTree("基本信息");
            eit_base.Children = new ObservableCollection<EntrySortInfoTree>();
            eit_base.Children.Add(new EntrySortInfoTree("业务日期"));
            eit_base.Children.Add(new EntrySortInfoTree("词条名称"));

            EntrySortInfoTree eit_property = new EntrySortInfoTree("属性");
            eit_property.Children = new ObservableCollection<EntrySortInfoTree>();
            var lpdbs = Core.Services.LabelPropertyService.Get1stLabel();
            foreach (var item in lpdbs)
                eit_property.Children.Add(new EntrySortInfoTree(item.Name));
            EntrySortInfoTrees.Add(eit_base);
            EntrySortInfoTrees.Add(eit_property);

            #endregion


        }
        private void InitEnumItemsource()
        {
            SortTypeStrs = new List<string>();
            SortWayStrs = new List<string>();
            foreach (Core.Enums.SortType p in Enum.GetValues(typeof(Core.Enums.SortType)))
            {
                SortTypeStrs.Add(p.GetDescription());
            }
            foreach (Core.Enums.SortWay p in Enum.GetValues(typeof(Core.Enums.SortWay)))
            {
                SortWayStrs.Add(p.GetDescription());
            }
        }
        public async Task UpdateEntryListAsync()
        {
            Helpers.InfoHelper.ShowWaiting();
            List<string> filterLabel = null;
            if (IsFilterLabel && Labels != null)
            {
                filterLabel = Labels.Where(p => p.IsChecked).Select(p => p.LabelDb.LCId).ToList();
            }
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType, SortWay, EntryStorages.Where(p => p.IsChecked).Select(p => p.StorageName).ToList(), filterLabel);
            if (queryResults?.Count > 0)
            {
                var newList = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p => p.ToQueryItem()).ToList());
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    Entries = newList.ToObservableCollection();
                });
            }
            else
            {
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    Entries = null;
                });
            }
            Helpers.InfoHelper.HideWaiting();
        }
        private async void UpdateSuggest(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                AutoSuggestItems = await Core.Services.EntryNameSerivce.QueryLikeNamesAsync(input);
            }
            else
            {
                AutoSuggestItems = null;
            }
        }
        private async void ConfirmSuggest(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var ls = await Core.Services.EntryNameSerivce.QueryFullNamesAsync(name);
                List<Core.Models.Entry> items = new List<Core.Models.Entry>();
                foreach (var p in ls.GroupBy(p => p.DbId))
                {
                    var entryIds = p.Select(p => p.Id).ToList().Distinct();
                    items.AddRange(await Core.Services.EntryService.GetEntryByIdsAsync(entryIds, p.Key));
                }
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    Entries = items.ToObservableCollection();
                });
            }
            else
            {
                _ = UpdateEntryListAsync();
            }
        }
        private void GlobalEvent_RemoveEntryEvent(object sender, Events.EntryEventArgs e)
        {
            if (Entries != null)
            {
                var item = Entries.FirstOrDefault(p => p.EntryId == e.Entry.EntryId);
                if (item != null)
                {
                    Entries.Remove(item);
                }
            }
        }
        private void GlobalEvent_AddEntryEvent(object sender, Events.EntryEventArgs e)
        {
            if (IsFitFilter(e.Entry))
            {
                if (Entries == null)
                {
                    Entries = new ObservableCollection<Entry>();
                }
                Entries.Add(e.Entry);
            }
        }
        private void GlobalEvent_UpdateEntryEvent(object sender, Events.EntryEventArgs e)
        {
            if (Entries != null)
            {
                var item = Entries.FirstOrDefault(p => p.EntryId == e.Entry.EntryId);
                if (item != null)
                {
                    int index = Entries.IndexOf(item);
                    Entries.Remove(item);
                    Entries.Insert(index, item);
                }
            }
        }
        private bool IsFitFilter(Entry entry)
        {
            var s = EntryStorages.Where(p => p.IsChecked).ToList();
            if (s != null && s.Count != 0)
            {
                if (s.FirstOrDefault(p => p.StorageName == entry.DbId) != null)
                {
                    if (!IsFilterLabel)
                    {
                        return true;
                    }
                    else
                    {
                        var labelIds = Core.Services.LabelClassService.GetLabelIdsOfEntry(entry.EntryId);
                        if (labelIds != null && labelIds.Count != 0)
                        {
                            var l = Labels.Where(p => p.IsChecked).ToList();
                            if (l != null && l.Count != 0)
                            {
                                return l.FirstOrDefault(p => labelIds.Contains(p.LabelDb.LCId)) != null;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion 
    }
}
