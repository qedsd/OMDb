using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class RecentlyWatchedFilesViewModel : ObservableObject
    {
        private ObservableCollection<Core.Models.RecentFile> recentlyWatchedFiles;
        public ObservableCollection<Core.Models.RecentFile> RecentlyWatchedFiles
        {
            get => recentlyWatchedFiles;
            set => SetProperty(ref recentlyWatchedFiles, value);
        }
        public Task InitAsync()
        {
            InitRecentlyWatchedFiles();
            return Task.CompletedTask;
        }

        private void InitRecentlyWatchedFiles()
        {
            RecentlyWatchedFiles = RecentFileService.RecentFiles;
        }

        public ICommand ItemClickCommand => new RelayCommand<Core.Models.RecentFile>((file) =>
        {
            if (file != null)
            {
                Helpers.FileHelper.OpenBySystem(file.Path);
            }
        });

        public ICommand ItemFolderCommand => new RelayCommand<Core.Models.RecentFile>((file) =>
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(file.Path));
        });
    }
}
