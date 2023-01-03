using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using OMDb.WinUI3.Extensions;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class ExtractLineViewModel : ObservableObject
    {
        private ImageSource lineCover;
        /// <summary>
        /// 台词封面
        /// </summary>
        public ImageSource LineCover
        {
            get => lineCover;
            set => SetProperty(ref lineCover, value);
        }

        private ExtractsLine extractsLine;
        /// <summary>
        /// 台词内容
        /// </summary>
        public ExtractsLine ExtractsLine
        {
            get => extractsLine;
            set
            {
                SetProperty(ref extractsLine, value);
            }
        }
        public async Task InitAsync()
        {
            await SetExtractsLine();
        }
        private async Task SetExtractsLine()
        {
            var ls = await Core.Services.EntryService.RandomEntryAsync(50);
            if (ls != null)
            {
                foreach (var entry in ls)
                {
                    var lines = entry.GetExtractsLines();
                    if (lines.NotNullAndEmpty())
                    {
                        entry.Name = Core.Services.EntryNameSerivce.QueryName(entry.Id, entry.DbId);
                        int lineIndex = Core.Helpers.RandomHelper.RandomInt(0, lines.Count - 1, 1).First();
                        ExtractsLine = Core.Models.ExtractsLine.Create(lines[lineIndex], entry);
                        var imgs = entry.GetBestImg(true);
                        if (imgs.NotNullAndEmpty())
                        {
                            Core.Helpers.ImageHelper.GetImageSize(imgs.First(), out var w, out var h);
                            if (w > 1080)
                            {
                                LineCover = await Helpers.ImgHelper.CreateBitmapImageAsync(await Core.Helpers.ImageHelper.ResetSizeAsync(imgs.First(), 1080, 0));
                            }
                            else
                            {
                                LineCover = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(imgs.First()));
                            }
                        }
                        return;
                    }
                }
            }
            ExtractsLine = null;
            LineCover = null;
        }

        public ICommand RefreshLineCommand => new RelayCommand(async () =>
        {
            Helpers.InfoHelper.ShowWaiting();
            await SetExtractsLine();
            Helpers.InfoHelper.HideWaiting();
        });

        public ICommand LineEntryDetailCommand => new RelayCommand(async () =>
        {
            Helpers.InfoHelper.ShowWaiting();
            var entry = await Core.Services.EntryService.QueryEntryAsync(new QueryItem(ExtractsLine.EntryId, ExtractsLine.DbId));
            NavigationService.Navigate(typeof(Views.EntryDetailPage), entry);
            Helpers.InfoHelper.HideWaiting();
        });
    }
}
