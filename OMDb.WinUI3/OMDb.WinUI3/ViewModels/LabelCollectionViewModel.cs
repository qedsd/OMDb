using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Services.DbServices;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Services;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    internal class LabelCollectionViewModel : ObservableObject
    {
        private List<Entry> entries;
        public List<Entry> Entries
        {
            get => entries;
            set
            {
                SetProperty(ref entries, value);
                ItemsSource = value;
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private string labelId;
        public string LabelId
        {
            get => labelId;
            set => SetProperty(ref labelId, value);
        }

        private List<Entry> itemsSource;
        public List<Entry> ItemsSource
        {
            get => itemsSource;
            set => SetProperty(ref itemsSource, value);
        }

        private Core.Enums.SortType sortType = Core.Enums.SortType.LastUpdateTime;
        public Core.Enums.SortType SortType
        {
            get => sortType;
            set
            {
                SetProperty(ref sortType, value);
                UpdateEntryList();
            }
        }
        private Core.Enums.SortWay sortWay = Core.Enums.SortWay.Positive;
        public Core.Enums.SortWay SortWay
        {
            get => sortWay;
            set
            {
                SetProperty(ref sortWay, value);
                UpdateEntryList();
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

        internal LabelCollectionViewModel()
        {
            InitEnumItemsource();
            sortType = Core.Enums.SortType.LastUpdateTime;
            sortWay = Core.Enums.SortWay.Positive;
        }

        private void InitEnumItemsource()
        {
            SortTypeStrs = new List<string>();
            SortWayStrs = new List<string>();
            foreach (Core.Enums.SortType p in System.Enum.GetValues(typeof(Core.Enums.SortType)))
            {
                SortTypeStrs.Add(p.GetDescription());
            }
            foreach (Core.Enums.SortWay p in System.Enum.GetValues(typeof(Core.Enums.SortWay)))
            {
                SortWayStrs.Add(p.GetDescription());
            }
        }

        private async void UpdateEntryList()
        {
            IEnumerable<Entry> items = null;
            Helpers.InfoHelper.ShowWaiting();
            switch (SortType)
            {
                case Core.Enums.SortType.CreateTime:
                    {
                        await Task.Run(() =>
                        {
                            items = SortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.CreateTime) : Entries.OrderByDescending(p => p.CreateTime);
                        });
                    }
                    break;
                case Core.Enums.SortType.LastWatchTime:
                    {
                        await Task.Run(() =>
                        {
                            items = SortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.LastWatchTime) : Entries.OrderByDescending(p => p.LastWatchTime);
                        });
                    }
                    break;
                case Core.Enums.SortType.MyRating:
                    {
                        await Task.Run(() =>
                        {
                            items = SortWay == Core.Enums.SortWay.Reverse ? Entries.OrderBy(p => p.MyRating) : Entries.OrderByDescending(p => p.MyRating);
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
                            items = SortWay == Core.Enums.SortWay.Positive ? Entries.OrderBy(p => p.LastUpdateTime) : Entries.OrderByDescending(p => p.LastUpdateTime);
                        });
                    }
                    break;
                default:items = Entries;break;
            }
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ItemsSource = items?.ToList();
            });
            Helpers.InfoHelper.HideWaiting();
        }
        private async Task<IEnumerable<Entry>> SortByWatchTimes()
        {
            List<Entry> sortedEntries = new List<Entry>();
            Dictionary<string, Entry> dic = Entries.ToDictionary(p => p.EntryId);
            List<QueryResult> queryResults = new List<QueryResult>();
            var groupEntries = Entries.GroupBy(p => p.DbId).ToList();
            foreach (var group in groupEntries)
            {
                var allIds = await EntryWatchHistoryService.QueryWatchHistoriesAsync(group.Select(p => p.EntryId).ToList(), group.Key);
                if(allIds != null && allIds.Count != 0)
                {
                    //有观看记录的
                    var orderGroups = allIds.GroupBy(p => p.EntryId).OrderBy(p => p.Count()).ToList();//分组key即为词条id
                    orderGroups.ForEach(p =>
                    {
                        queryResults.Add(new QueryResult(p.Key, p.Count(), group.Key));
                    });
                }
            }
            List<QueryResult> order;
            if (SortWay == Core.Enums.SortWay.Positive)
            {
                order = queryResults.OrderByDescending(p => p.Value).ToList();
            }
            else
            {
                order = queryResults.OrderBy(p => p.Value).ToList();
            }
            //按指定正反排序有观看记录的
            foreach (var orderGroup in order)
            {
                if (dic.TryGetValue(orderGroup.Id, out var entry))
                {
                    sortedEntries.Add(entry);
                    dic.Remove(orderGroup.Id);
                }
            }
            if(dic.Count != 0)
            {
                //剩下的为无观看记录的
                if (SortWay == Core.Enums.SortWay.Positive)
                {
                    //直接插在后面
                    sortedEntries.AddRange(dic.Values);
                }
                else
                {
                    //插在开头
                    var unwatcheds = dic.Values.ToList();
                    unwatcheds.AddRange(sortedEntries);
                    sortedEntries = unwatcheds;
                }
            }
            return sortedEntries;
        }

        public ICommand ItemClickCommand => new RelayCommand<Core.Models.Entry>((entry) =>
        {
            TabViewService.AddItem(new Views.EntryDetailPage(entry));
        });
    }
}
