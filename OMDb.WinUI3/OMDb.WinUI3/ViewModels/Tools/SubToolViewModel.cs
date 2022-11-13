using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels.Tools
{
    internal class SubToolViewModel : ObservableObject
    {
        //private string filePath;
        //public string FilePath
        //{
        //    get => filePath;
        //    set => SetProperty(ref filePath, value);
        //}

        //private int width;
        //public int Width
        //{
        //    get => width;
        //    set => SetProperty(ref width, value);
        //}

        //private int height;
        //public int Height
        //{
        //    get => height;
        //    set => SetProperty(ref height, value);
        //}

        //private TimeSpan duration;
        //public TimeSpan Duration
        //{
        //    get => duration;
        //    set => SetProperty(ref duration, value);
        //}

        //private string pixelFormat;
        //public string PixelFormat
        //{
        //    get => pixelFormat;
        //    set => SetProperty(ref pixelFormat, value);
        //}

        //private long size;
        //public long Size
        //{
        //    get => size;
        //    set => SetProperty(ref size, value);
        //}

        private bool infoPanelVisible;
        public bool InfoPanelVisible
        {
            get => infoPanelVisible;
            set => SetProperty(ref infoPanelVisible, value);
        }

        private Core.Models.MediaInfo mediaInfo;
        public Core.Models.MediaInfo MediaInfo
        {
            get => mediaInfo;
            set 
            {
                SetProperty(ref mediaInfo, value);
                InfoPanelVisible = value == null? false : true;
            }
        }

        public SubToolViewModel()
        {
        }

        public ICommand SelecteFileCommand => new RelayCommand(async() =>
        {
            var file = await Helpers.PickHelper.PickFileAsync();
            if(file != null)
            {
                MediaInfo = await Core.Helpers.FFmpegHelper.GetMediaInfoAsync(file.Path);
            }
        });
    }
}
