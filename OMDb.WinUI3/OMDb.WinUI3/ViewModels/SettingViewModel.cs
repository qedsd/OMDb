using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Dialogs;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.Services.Settings;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace OMDb.WinUI3.ViewModels
{
    public class SettingViewModel : ObservableObject
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }


        private int selectedThemeIndex = (int)ThemeSelectorService.Theme;
        public int SelectedThemeIndex
        {
            get => selectedThemeIndex;
            set
            {
                selectedThemeIndex = value;
                ElementTheme = (ElementTheme)value;
                _ = ThemeSelectorService.SetThemeAsync(ElementTheme);
            }
        }

        private string potPlayerPlaylistPath = PotPlayerPlaylistSelectorService.PlaylistPath;
        public string PotPlayerPlaylistPath
        {
            get => potPlayerPlaylistPath;
            set
            {
                SetProperty(ref potPlayerPlaylistPath, value);
                _ = PotPlayerPlaylistSelectorService.SetAsync(value);
            }
        }

        public ICommand PickPotPlayerPlaylistFileCommand => new RelayCommand(async () =>
        {
            var file = await Helpers.PickHelper.PickFileAsync(".dpl");
            if (file != null && !string.IsNullOrEmpty(file.Path))
            {
                PotPlayerPlaylistPath = file.Path;
                RecentFileService.Init();
            }
        });


        private ObservableCollection<Models.HomeItemConfig> inactiveHomeItems = HomeItemConfigsService.InactiveItems.ToObservableCollection();
        public ObservableCollection<Models.HomeItemConfig> InactiveHomeItems
        {
            get => inactiveHomeItems;
            set => SetProperty(ref inactiveHomeItems, value);
        }
        private ObservableCollection<Models.HomeItemConfig> activeHomeItems = HomeItemConfigsService.ActiveItems.ToObservableCollection();
        public ObservableCollection<Models.HomeItemConfig> ActiveHomeItems
        {
            get => activeHomeItems;
            set => SetProperty(ref activeHomeItems, value);
        }

        public SettingViewModel()
        {
            ActiveHomeItems.CollectionChanged += ActiveHomeItems_CollectionChanged;
            LoadDbs();
        }

        private async void ActiveHomeItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await HomeItemConfigsService.SetAsync(ActiveHomeItems.ToList());
        }

        public ICommand DropInactiveHomeItemCommand => new RelayCommand<Models.HomeItemConfig>((item) =>
        {

        });
        public ICommand DropActiveHomeItemCommand => new RelayCommand<Models.HomeItemConfig>((item) =>
        {

        });



        public ICommand DbSelector_Refresh => new RelayCommand(() =>
        {
            LoadDbs();
        });

        public ICommand DbSelector_Add => new RelayCommand(async () =>
        {
            var dbName = await EditDbSource.ShowDialog();

            if ((dbName == null || dbName.Count() == 0))
            {
                Helpers.InfoHelper.ShowError("请输入DbName");
            }
            else if (DbsCollection.Select(a => a.DbSourceDb.DbName).ToList().Contains(dbName))
            {
                Helpers.InfoHelper.ShowError("已存在同名DbSource");
            }
            else
            {
                DbSelectorService.AddDbAsync(dbName);
                LoadDbs();
            }
        });

        public ICommand DbSelector_Edit => new RelayCommand<string>(async (dbName) =>
        {
            dbName = await EditDbSource.ShowDialog(dbName);

            if (dbName == null || dbName.Count() == 0)
            {
                Helpers.InfoHelper.ShowError("请输入DbName");
            }
            else if (DbsCollection.Select(a => a.DbSourceDb.DbName).ToList().Contains(dbName))
            {
                Helpers.InfoHelper.ShowError("已存在同名DbSource");
            }
            else
            {
                DbSelectorService.AddDbAsync(dbName);
                LoadDbs();
            }
        });



        public ICommand DbSelector_Save => new RelayCommand(async () =>
        {
            await DbSelectorService.SetAsync(DbCurrent.DbSourceDb.Id);
            LoadDbs();
        });

        private void LoadDbs()
        {
            Services.Settings.DbSelectorService.Initialize();
            if(DbsCollection==null) DbsCollection = new ObservableCollection<DbSource>();
            DbsCollection.Clear();
            foreach (var item in DbSelectorService.dbsCollection)
            {
                DbsCollection.Add(item);
            }
            DbCurrent = DbsCollection.Where(a=>a.DbSourceDb.Id==DbSelectorService.dbCurrentId).FirstOrDefault();
        }


        public ObservableCollection<DbSource> _dbsCollection;
        public ObservableCollection<DbSource> DbsCollection
        {
            get => _dbsCollection;
            set => SetProperty(ref _dbsCollection, value);
        }

        private DbSource _dbCurrent;

        public DbSource DbCurrent
        {
            get => _dbCurrent;
            set => SetProperty(ref _dbCurrent, value);
        }


        public ICommand DbSelector_Delete => new RelayCommand<DbSource>(async (db) =>
        {
            var flag = await Dialogs.QueryDialog.ShowDialog("再次确认", "请确认是否删除");
            if (flag)
            {
                DbSelectorService.RemoveDbAsync(db.DbSourceDb.Id);
                DbsCollection.Remove(db);
                LoadDbs();
            }
            else return;
        });


    }
}
