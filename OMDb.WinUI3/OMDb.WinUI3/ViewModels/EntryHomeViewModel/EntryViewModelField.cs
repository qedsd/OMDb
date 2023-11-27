using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Models.EntryModels;
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

    /// <summary>
    /// 字段
    /// </summary>
    public partial class EntryHomeViewModel : ObservableObject
    {
        #region 字段
        public ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages;

        private ObservableCollection<Core.Models.Entry> _entriesAll;
        public ObservableCollection<Core.Models.Entry> EntriesAll
        {
            get => _entriesAll;
            set
            {
                SetProperty(ref _entriesAll, value);
            }
        }

        private ObservableCollection<Core.Models.Entry> _entries;
        public ObservableCollection<Core.Models.Entry> Entries
        {
            get => _entries;
            set
            {
                SetProperty(ref _entries, value);
            }
        }
        private ObservableCollection<LabelClass> _labels;
        public ObservableCollection<LabelClass> Labels
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
                UpdateEntryListAsync();
            }
        }
        private Core.Enums.SortWay sortWay = Core.Enums.SortWay.Positive;
        public Core.Enums.SortWay SortWay
        {
            get => sortWay;
            set
            {
                SetProperty(ref sortWay, value);
                UpdateEntryListAsync();
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
        private bool isFilterLabelClass = false;
        /// <summary>
        /// 是否按选择的标签进行筛选
        /// </summary>
        public bool IsFilterLabelClass
        {
            get => isFilterLabelClass;
            set
            {
                SetProperty(ref isFilterLabelClass, value);
                UpdateEntryListAsync();
            }
        }

        private bool isFilterLabelProperty = false;
        /// <summary>
        /// 是否按选择的标签进行筛选
        /// </summary>
        public bool IsFilterLabelProperty
        {
            get => isFilterLabelProperty;
            set
            {
                SetProperty(ref isFilterLabelProperty, value);
                UpdateEntryListAsync();
            }
        }

        private ObservableCollection<EntrySortInfoTree> _entrySortInfoTrees = new ObservableCollection<EntrySortInfoTree>();
        public ObservableCollection<EntrySortInfoTree> EntrySortInfoTrees
        {
            get => _entrySortInfoTrees;
            set => SetProperty(ref _entrySortInfoTrees, value);
        }

        private EntrySortInfoTree _entrySortInfoCurrent;
        public EntrySortInfoTree EntrySortInfoCurrent
        {
            get => _entrySortInfoCurrent;
            set => SetProperty(ref _entrySortInfoCurrent, value);
        }


        private ObservableCollection<EntrySortInfoResult> _entrySortInfoResults = new ObservableCollection<EntrySortInfoResult>();
        public ObservableCollection<EntrySortInfoResult> EntrySortInfoResults
        {
            get => _entrySortInfoResults;
            set => SetProperty(ref _entrySortInfoResults, value);
        }

        private EntrySortInfoResult _entrySortInfoResultCurrent;
        public EntrySortInfoResult EntrySortInfoResultCurrent
        {
            get => _entrySortInfoResultCurrent;
            set => SetProperty(ref _entrySortInfoResultCurrent, value);
        }

        private ObservableCollection<LabelClassTree> _labelClassTrees = new ObservableCollection<LabelClassTree>();
        public ObservableCollection<LabelClassTree> LabelClassTrees
        {
            get => _labelClassTrees;
            set => SetProperty(ref _labelClassTrees, value);
        }

        private ObservableCollection<LabelPropertyTree> _labelPropertyTrees = new ObservableCollection<LabelPropertyTree>();
        public ObservableCollection<LabelPropertyTree> LabelPropertyTrees
        {
            get => _labelPropertyTrees;
            set => SetProperty(ref _labelPropertyTrees, value);
        }

        private int _minTime;
        public int MinTime
        {
            get => _minTime;
            set
            {
                value = value <= 0 ? 0 : value;
                SetProperty(ref _minTime, value);
            }
        }
        private int _maxTime;
        public int MaxTime
        {
            get => _maxTime;
            set
            {
                value = value >= 275 ? 275 : value;
                SetProperty(ref _maxTime, value);
            }
        }

        private double _minRank;
        public double MinRank
        {
            get => _minRank;
            set 
            {
                SetProperty(ref _minRank, value);
                UpdateEntrySingle();
            }
        }

        private MaxMinDateModel _maxMinDate;
        public MaxMinDateModel MaxMinDate
        {
            get => _maxMinDate;
            set => SetProperty(ref _maxMinDate, value);
        }
        #endregion
    }
}
