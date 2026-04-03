using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    public partial class EntryCollectionDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private Core.Models.EntryCollection _entryCollection;

        [ObservableProperty]
        private ObservableCollection<Core.Models.EntryCollectionItem> _itemsSource;

        [ObservableProperty]
        private Core.Enums.SortType _sortType = Core.Enums.SortType.LastUpdateTime;

        [ObservableProperty]
        private Core.Enums.SortWay _sortWay = Core.Enums.SortWay.Positive;

        public List<string> SortTypeStrs { get; set; }

        [ObservableProperty]
        private int _sortTypeIndex = 0;

        public List<string> SortWayStrs { get; set; }

        [ObservableProperty]
        private int _sortWayIndex = 0;

        [ObservableProperty]
        private string _editTitle = string.Empty;

        [ObservableProperty]
        private string _editDesc = string.Empty;

        public EntryCollectionDetailViewModel()
        {
            InitEnumItemsource();
        }

        private void InitEnumItemsource()
        {
            SortTypeStrs = new List<string>()
            {
                "加入时间"
            };
            SortWayStrs = new List<string>();
            foreach (Core.Enums.SortWay p in Enum.GetValues(typeof(Core.Enums.SortWay)))
            {
                SortWayStrs.Add(p.ToString());
            }
        }

        partial void OnEntryCollectionChanged(Core.Models.EntryCollection oldValue, Core.Models.EntryCollection newValue)
        {
            Init();
        }

        private async void Init()
        {
            if (EntryCollection == null)
            {
                return;
            }

            if (EntryCollection.Items != null && EntryCollection.Items.Count != 0)
            {
                if (EntryCollection.Items.FirstOrDefault(p => p.Entry == null) != null)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(EntryCollection.Items.Select(p => p.ToQueryItem()).ToList());
                    if (entrys != null && entrys.Count != 0)
                    {
                        await Task.Run(() =>
                        {
                            var dic = entrys.ToDictionary(p => p.EntryId);
                            foreach (var item in EntryCollection.Items)
                            {
                                if (dic.TryGetValue(item.EntryId, out var entry))
                                {
                                    item.Entry = entry;
                                }
                            }
                        });
                    }
                }
            }
            EditTitle = EntryCollection.Title ?? string.Empty;
            EditDesc = EntryCollection.Description ?? string.Empty;
            UpdateEntryList();
        }

        partial void OnSortTypeChanged(Core.Enums.SortType oldValue, Core.Enums.SortType newValue)
        {
            UpdateEntryList();
        }

        partial void OnSortWayChanged(Core.Enums.SortWay oldValue, Core.Enums.SortWay newValue)
        {
            UpdateEntryList();
        }

        private async void UpdateEntryList()
        {
            if (EntryCollection == null || EntryCollection.Items == null || EntryCollection.Items.Count == 0)
            {
                return;
            }

            ItemsSource?.Clear();
            ItemsSource = null;
            if (SortWay == Core.Enums.SortWay.Positive)
            {
                ItemsSource = new ObservableCollection<Core.Models.EntryCollectionItem>(EntryCollection.Items.OrderBy(p => p.AddTime).ToList());
            }
            else
            {
                ItemsSource = new ObservableCollection<Core.Models.EntryCollectionItem>(EntryCollection.Items.OrderByDescending(p => p.AddTime).ToList());
            }
        }

        [RelayCommand]
        private void ItemClick(Core.Models.EntryCollectionItem item)
        {
            // TODO: 导航到 EntryDetailPage
        }

        [RelayCommand]
        private void ConfirmEdit()
        {
            if (EntryCollection != null)
            {
                EntryCollection.Title = EditTitle ?? string.Empty;
                EntryCollection.Description = EditDesc ?? string.Empty;
                EntryCollection.LastUpdateTime = DateTime.Now;
                Core.Services.EntryCollectionService.UpdateCollection(EntryCollection.ToEntryCollectionDb());
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            if (EntryCollection != null)
            {
                EditTitle = EntryCollection.Title ?? string.Empty;
                EditDesc = EntryCollection.Description ?? string.Empty;
            }
        }

        [RelayCommand]
        private async Task Remove(List<Core.Models.EntryCollectionItem> list)
        {
            if (list != null && list.Count != 0 && EntryCollection != null && ItemsSource != null)
            {
                // TODO: 显示确认对话框
                Core.Services.EntryCollectionService.RemoveCollectionItem(list.Select(p => p.Id).ToList());
                foreach (var item in list)
                {
                    ItemsSource.Remove(item);
                    EntryCollection.Items.Remove(item);
                }
            }
        }

        [RelayCommand]
        private async Task RemoveOne(Core.Models.EntryCollectionItem item)
        {
            if (item != null && EntryCollection != null && ItemsSource != null)
            {
                // TODO: 显示确认对话框
                if (Core.Services.EntryCollectionService.RemoveCollectionItem(item.Id))
                {
                    ItemsSource.Remove(item);
                    EntryCollection.Items.Remove(item);
                }
            }
        }
    }
}
