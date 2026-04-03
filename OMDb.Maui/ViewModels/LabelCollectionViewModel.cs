using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoreEntry = OMDb.Core.Models.Entry;
using OMDb.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    public partial class LabelCollectionViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<CoreEntry> _entries;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _labelId;

        [ObservableProperty]
        private List<CoreEntry> _itemsSource;

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

        public LabelCollectionViewModel()
        {
            InitEnumItemsource();
            _sortType = Core.Enums.SortType.LastUpdateTime;
            _sortWay = Core.Enums.SortWay.Positive;
        }

        private void InitEnumItemsource()
        {
            SortTypeStrs = new List<string>();
            SortWayStrs = new List<string>();
            foreach (Core.Enums.SortType p in Enum.GetValues(typeof(Core.Enums.SortType)))
            {
                SortTypeStrs.Add(p.ToString());
            }
            foreach (Core.Enums.SortWay p in Enum.GetValues(typeof(Core.Enums.SortWay)))
            {
                SortWayStrs.Add(p.ToString());
            }
        }

        partial void OnEntriesChanged(List<CoreEntry> oldValue, List<CoreEntry> newValue)
        {
            ItemsSource = newValue;
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
            IEnumerable<CoreEntry> items = null;
            switch (_sortType)
            {
                case Core.Enums.SortType.CreateTime:
                    {
                        await Task.Run(() =>
                        {
                            items = _sortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.CreateTime) : Entries.OrderByDescending(p => p.CreateTime);
                        });
                    }
                    break;
                case Core.Enums.SortType.LastWatchTime:
                    {
                        await Task.Run(() =>
                        {
                            items = _sortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.LastWatchTime) : Entries.OrderByDescending(p => p.LastWatchTime);
                        });
                    }
                    break;
                case Core.Enums.SortType.MyRating:
                    {
                        await Task.Run(() =>
                        {
                            items = _sortWay == Core.Enums.SortWay.Reverse ? Entries.OrderBy(p => p.MyRating) : Entries.OrderByDescending(p => p.MyRating);
                        });
                    }
                    break;
                case Core.Enums.SortType.WatchTimes:
                    {
                        items = await SortByWatchTimes();
                    }
                    break;
                case Core.Enums.SortType.LastUpdateTime:
                    {
                        await Task.Run(() =>
                        {
                            items = _sortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.LastUpdateTime) : Entries.OrderByDescending(p => p.LastUpdateTime);
                        });
                    }
                    break;
                default:
                    items = Entries;
                    break;
            }
            ItemsSource = items?.ToList();
        }

        private async Task<IEnumerable<CoreEntry>> SortByWatchTimes()
        {
            List<CoreEntry> sortedEntries = new List<CoreEntry>();
            Dictionary<string, CoreEntry> dic = Entries.ToDictionary(p => p.EntryId);
            List<Core.Models.QueryResult> queryResults = new List<Core.Models.QueryResult>();
            var groupEntries = Entries.GroupBy(p => p.DbId).ToList();
            foreach (var group in groupEntries)
            {
                var allIds = await EntryWatchHistoryService.QueryWatchHistoriesAsync(group.Select(p => p.EntryId).ToList(), group.Key);
                if (allIds != null && allIds.Count != 0)
                {
                    var orderGroups = allIds.GroupBy(p => p.EntryId).OrderBy(p => p.Count()).ToList();
                    orderGroups.ForEach(p =>
                    {
                        queryResults.Add(new Core.Models.QueryResult(p.Key, p.Count(), group.Key));
                    });
                }
            }
            List<Core.Models.QueryResult> order;
            if (_sortWay == Core.Enums.SortWay.Positive)
            {
                order = queryResults.OrderByDescending(p => p.Value).ToList();
            }
            else
            {
                order = queryResults.OrderBy(p => p.Value).ToList();
            }
            foreach (var orderGroup in order)
            {
                if (dic.TryGetValue(orderGroup.Id, out var entry))
                {
                    sortedEntries.Add(entry);
                    dic.Remove(orderGroup.Id);
                }
            }
            if (dic.Count != 0)
            {
                var unwatcheds = dic.Values.ToList();
                if (_sortWay == Core.Enums.SortWay.Positive)
                {
                    sortedEntries.AddRange(unwatcheds);
                }
                else
                {
                    unwatcheds.AddRange(sortedEntries);
                    sortedEntries = unwatcheds;
                }
            }
            return sortedEntries;
        }

        [RelayCommand]
        private void ItemClick(Core.Models.Entry entry)
        {
            // TODO: 导航到 EntryDetailPage
        }
    }
}
