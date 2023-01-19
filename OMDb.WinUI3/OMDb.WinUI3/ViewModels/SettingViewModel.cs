using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using OMDb.Core.Extensions;
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

        public ICommand DbSelector_Add => new RelayCommand(async() =>
        {
            await DbSelectorService.AddDbAsync("omdb4th");
            LoadDbs();
        });

        public ICommand DbSelector_Save => new RelayCommand(async() =>
        {
            await DbSelectorService.SetAsync(DbCurrent);
            LoadDbs();
        });

        private void LoadDbs()
        {
            Services.Settings.DbSelectorService.Initialize();
            DbCurrent = DbSelectorService.dbCurrent;
            DbsCollection = new ObservableCollection<string>();
            DbsCollection.Clear();
            foreach (var item in DbSelectorService.dbsCollection)
            {
                if (!DbsCollection.Contains(item))
                {
                    DbsCollection.Add(item);
                }
            }
        }

        public ObservableCollection<string> _dbsCollection;
        public ObservableCollection<string> DbsCollection
        {
            get => _dbsCollection;
            set => SetProperty(ref _dbsCollection, value);
        }

        private string _dbCurrent;

        public string DbCurrent
        {
            get => _dbCurrent;
            set => SetProperty(ref _dbCurrent, value);
        }
    }
}
