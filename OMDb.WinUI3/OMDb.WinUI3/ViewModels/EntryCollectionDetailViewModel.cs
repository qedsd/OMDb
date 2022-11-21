using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using OMDb.Core.Extensions;
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
    internal class EntryCollectionDetailViewModel : ObservableObject
    {
        private EntryCollection entryCollection;
        public EntryCollection EntryCollection
        {
            get => entryCollection;
            set
            {
                SetProperty(ref entryCollection, value);
                Init();
            }
        }

        private List<Core.Models.Entry> itemsSource;
        public List<Core.Models.Entry> ItemsSource
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

        private string editTitle = string.Empty;
        public string EditTitle
        {
            get => editTitle;
            set => SetProperty(ref editTitle, value);
        }

        private string editDesc = string.Empty;
        public string EditDesc
        {
            get => editDesc;
            set => SetProperty(ref editDesc, value);
        }

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
            foreach (Core.Enums.SortWay p in System.Enum.GetValues(typeof(Core.Enums.SortWay)))
            {
                SortWayStrs.Add(p.GetDescription());
            }
        }

        private Dictionary<Core.DbModels.EntryCollectionItemDb, Core.Models.Entry> KeyValuePairs;
        private async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();
            KeyValuePairs = new Dictionary<Core.DbModels.EntryCollectionItemDb, Core.Models.Entry>();
            if(EntryCollection.Items != null && EntryCollection.Items.Count != 0)
            {
                var entrys = await Core.Services.EntryService.QueryEntryAsync(EntryCollection.Items.Select(p => p.ToQueryItem()).ToList());
                if (entrys != null && entrys.Count != 0)
                {
                    await Task.Run(() =>
                    {
                        var dic = entrys.ToDictionary(p => p.Id);
                        foreach (var item in EntryCollection.Items)
                        {
                            if (dic.TryGetValue(item.EntryId, out var entry))
                            {
                                KeyValuePairs.Add(item, entry);
                            }
                        }
                    });
                }
            }
            EditTitle = EntryCollection.Title;
            EditDesc = EntryCollection.Description;
            UpdateEntryList();
            Helpers.InfoHelper.HideWaiting();
        }
        private async void UpdateEntryList()
        {
            ItemsSource?.Clear();
            ItemsSource = null;
            if (SortWay == Core.Enums.SortWay.Positive)
            {
                ItemsSource = await Task.Run(() =>
                {
                    return KeyValuePairs.OrderBy(p => p.Key.AddTime).Select(p => p.Value).ToList();
                });
            }
        }

        public static ICommand ItemClickCommand => new RelayCommand<Core.Models.Entry>((entry) =>
        {
            NavigationService.Navigate(typeof(Views.EntryDetailPage), entry);
        });

        public ICommand ConfirmEditCommand => new RelayCommand(() =>
        {
            EntryCollection.Title = EditTitle;
            EntryCollection.Description = EditDesc;
            EntryCollection.LastUpdateTime = DateTime.Now;
            Core.Services.EntryCollectionService.UpdateCollection(EntryCollection.ToEntryCollectionDb());
            Helpers.InfoHelper.ShowSuccess("已更新");
        });
        public ICommand CancelEditCommand => new RelayCommand(() =>
        {
            EditTitle = EntryCollection.Title;
            EditDesc = EntryCollection.Description;
        });
    }
}
