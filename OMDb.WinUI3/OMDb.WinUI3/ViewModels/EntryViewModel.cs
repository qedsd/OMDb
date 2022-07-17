using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
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
    public class EntryViewModel : ObservableObject
    {
        public ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages;
        private List<Core.Models.Entry> entries;
        public List<Core.Models.Entry> Entries
        {
            get => entries;
            set
            {
                SetProperty(ref entries, value);
            }
        }
        private ObservableCollection<Core.DbModels.LabelDb> labelDbs;
        public ObservableCollection<Core.DbModels.LabelDb> LabelDbs
        {
            get => labelDbs;
            set => SetProperty(ref labelDbs, value);
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
        public EntryViewModel()
        {
            InitEnumItemsource();
            Init();
        }
        private async void Init()
        {
            sortType = Core.Enums.SortType.LastUpdateTime;
            sortWay = Core.Enums.SortWay.Positive;
            await InitEntry();
            await InitLabel();
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
        private async Task InitEntry()
        {
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType, SortWay);
            if(queryResults?.Count > 0)
            {
                Entries = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p=>p.ToQueryItem()).ToList());
            }
        }
        private async Task InitLabel ()
        {
            var labels = await Core.Services.LabelService.GetAllLabelAsync();
            if (labels != null)
            {
                Labels = labels.Select(p=>new Label(p)).ToList();
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    LabelDbs = new ObservableCollection<Core.DbModels.LabelDb>(labels);
                });
            }
        }
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
            Helpers.InfoHelper.ShowMsg("刷新完成");
        });
        public ICommand ItemClickCommand => new RelayCommand<Core.Models.Entry>((entry) =>
        {
            NavigationService.Navigate(typeof(Views.EntryDetailPage), entry);
        });
        private List<Models.Label> Labels;
        public ICommand LabelSelectedCommand => new RelayCommand<Models.Label>((label) =>
        {
            Labels.FirstOrDefault(p=>p.LabelDb == label.LabelDb).IsChecked = label.IsChecked;
            UpdateEntryList();
        });
        private async void UpdateEntryList()
        {
            List<string> filterLabel = null;
            if(Labels!=null)
            {
                filterLabel = Labels.Where(p=>p.IsChecked).Select(p=>p.LabelDb.Id).ToList();
            }
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType, SortWay, filterLabel);
            if (queryResults?.Count > 0)
            {
                var newList = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p => p.ToQueryItem()).ToList());
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    Entries = newList;
                });
            }
        }
    }
}
