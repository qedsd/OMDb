using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Utils;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public partial class EntryHomeViewModel : ObservableObject
    {
        public EntryHomeViewModel()
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
        public static EntryHomeViewModel Current { get; private set; }

        #region Methon
        private async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();

            #region 筛选信息加载
            var labelDbs = await Core.Services.LabelClassService.GetAllLabelAsync(Services.Settings.DbSelectorService.dbCurrentId);
            List<LabelClass> LabelClasses = null;
            if (labelDbs != null)
            {
                LabelClasses = labelDbs.Select(p => new LabelClass(p)).ToList();
                Labels = new ObservableCollection<LabelClass>(LabelClasses);
            }
            EntryStorages = ConfigService.EnrtyStorages;
            foreach (var item in EntryStorages)
            {
                item.IsChecked = true;
            }

            this.LabelClassTrees = Services.CommonService.GetLabelClassTrees().Result.ToObservableCollection();
            this.LabelPropertyTrees = Services.CommonService.GetLabelPropertyTrees().Result.ToObservableCollection();

            /*foreach (var LabelClass in LabelClasses)
            {
                if (LabelClass.LabelClassDb.ParentId == null)
                {
                    LabelClassTrees.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCId == LabelClass.LabelClassDb.LCId).LabelClass.IsChecked = true;
                }
                else
                {
                    var lc = LabelClassTrees.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCId == LabelClass.LabelClassDb.ParentId);
                    lc.Children.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCId == LabelClass.LabelClassDb.LCId).LabelClass.IsChecked = true;
                }
            }*/


            await UpdateEntryListAsync();
            #endregion

            #region 排序模式加载

            EntrySortInfoTree eitBase = new EntrySortInfoTree("基本信息", null);
            eitBase.Children = new ObservableCollection<EntrySortInfoTree>();
            eitBase.Children.Add(new EntrySortInfoTree("业务日期", "Base"));
            eitBase.Children.Add(new EntrySortInfoTree("词条名称", "Base"));

            EntrySortInfoTree eitLabelProperty = new EntrySortInfoTree("属性标签", null);
            eitLabelProperty.Children = new ObservableCollection<EntrySortInfoTree>();//初始化
            var lpdbs = Core.Services.LabelPropertyService.Get1stLabel();
            foreach (var item in lpdbs)
                eitLabelProperty.Children.Add(new EntrySortInfoTree(item.Name, "LabelProperty"));

            EntrySortInfoTree eitLabelClass = new EntrySortInfoTree("分类标签", null);
            eitLabelClass.Children = new ObservableCollection<EntrySortInfoTree>();//初始化
            var lcdbs = Core.Services.LabelClassService.Get1stLabel();
            foreach (var item in lcdbs)
                eitLabelClass.Children.Add(new EntrySortInfoTree(item.Name, "LabelClass"));

            //加载待排序树
            EntrySortInfoTrees.Add(eitBase);
            EntrySortInfoTrees.Add(eitLabelProperty);
            EntrySortInfoTrees.Add(eitLabelClass);

            #endregion

            Helpers.InfoHelper.HideWaiting();
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

            var storageFilterList = EntryStorages.Where(p => p.IsChecked).Select(p => p.StorageName).ToList();
            var labelClassFilterList = GetLabelClassId() ;
            var labelPrpertyFilterList =  GetLabelPropertyId();

            
            var sortModel = new SortModel(this.SortType,this.SortWay);
            var filterModel=new FilterModel();
            filterModel.IsFilterStorage = true;
            filterModel.IsFilterLabelClass = IsFilterLabelClass;
            filterModel.IsFilterLabelProperty = IsFilterLabelProperty;
            filterModel.StorageIds = storageFilterList;
            filterModel.LabelClassIds = labelClassFilterList;
            filterModel.LabelPropertyIds= labelPrpertyFilterList;
            
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);


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
                await UpdateEntryListAsync();
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
                    if (!IsFilterLabelClass)
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
                                return l.FirstOrDefault(p => labelIds.Contains(p.LabelClassDb.LCId)) != null;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private List<string> GetLabelClassId()
        {
            var labelFilterList = new List<string>();
            foreach (var labelClass1st in this.LabelClassTrees)
            {
                if (labelClass1st.LabelClass.IsChecked)
                    labelFilterList.Add(labelClass1st.LabelClass.LabelClassDb.LCId);


                foreach (var lableClass2nd in labelClass1st.Children)
                {
                    if (lableClass2nd.LabelClass.IsChecked)
                        labelFilterList.Add(lableClass2nd.LabelClass.LabelClassDb.LCId);
                }
            }
            return labelFilterList;
        }


        private List<string> GetLabelPropertyId()
        {
            var labelFilterList = new List<string>();
            foreach (var labelProperty1st in this.LabelPropertyTrees)
            {
                if (labelProperty1st.LabelProperty.IsChecked)
                    labelFilterList.Add(labelProperty1st.LabelProperty.LPDb.LPId);


                foreach (var lableClass2nd in labelProperty1st.Children)
                {
                    if (lableClass2nd.LabelProperty.IsChecked)
                        labelFilterList.Add(lableClass2nd.LabelProperty.LPDb.LPId);
                }
            }
            return labelFilterList;
        }

        #endregion 
    }
}
