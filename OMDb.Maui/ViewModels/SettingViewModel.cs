using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.DbModels.ManagerCenterDb;
using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    public partial class SettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _selectedThemeIndex;

        [ObservableProperty]
        private string _potPlayerPlaylistPath = string.Empty;

        [ObservableProperty]
        private ObservableCollection<OMDb.Maui.Models.HomeItemConfig> _inactiveHomeItems;

        [ObservableProperty]
        private ObservableCollection<OMDb.Maui.Models.HomeItemConfig> _activeHomeItems;

        public ObservableCollection<DbCenterDb> _dbsCollection;
        public ObservableCollection<DbCenterDb> DbsCollection
        {
            get => _dbsCollection;
            set => SetProperty(ref _dbsCollection, value);
        }

        private DbCenterDb _dbCurrent;
        public DbCenterDb DbCurrent
        {
            get => _dbCurrent;
            set => SetProperty(ref _dbCurrent, value);
        }

        public SettingViewModel()
        {
            ActiveHomeItems = new ObservableCollection<Models.HomeItemConfig>();
            InactiveHomeItems = new ObservableCollection<Models.HomeItemConfig>();
            ActiveHomeItems.CollectionChanged += ActiveHomeItems_CollectionChanged;
            LoadDbs();
        }

        private async void ActiveHomeItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Run(() => {
                // TODO: 保存主页部件配置
            });
        }

        [RelayCommand]
        private void DropInactiveHomeItem(Models.HomeItemConfig item)
        {
        }

        [RelayCommand]
        private void DropActiveHomeItem(Models.HomeItemConfig item)
        {
        }

        [RelayCommand]
        private void DbSelector_Refresh()
        {
            LoadDbs();
        }

        [RelayCommand]
        private async Task DbSelector_Add()
        {
            // TODO: 显示添加数据库对话框
            LoadDbs();
        }

        [RelayCommand]
        private async Task DbSelector_Save()
        {
            // TODO: 保存数据库选择
        }

        private void LoadDbs()
        {
            // TODO: 实现数据库加载逻辑
            if (DbsCollection == null)
                DbsCollection = new ObservableCollection<DbCenterDb>();
            DbsCollection.Clear();
        }

        [RelayCommand]
        private async Task PickPotPlayerPlaylistFile()
        {
            // TODO: 实现文件选择器
        }
    }
}
