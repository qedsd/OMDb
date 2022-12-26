using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
